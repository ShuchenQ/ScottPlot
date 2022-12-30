﻿namespace ScottPlot.Axis.StandardAxes;

public abstract class XAxisBase : IAxis
{
    public double Min
    {
        get => Range.Min;
        set => Range.Min = value;
    }

    public double Max
    {
        get => Range.Max;
        set => Range.Max = value;
    }

    public double Width => Range.Span;

    public virtual CoordinateRange Range { get; private set; } = CoordinateRange.NotSet;

    public ITickGenerator TickGenerator { get; set; } = null!;

    public Label Label { get; private set; } = new()
    {
        Text = string.Empty,
        FontName = FontService.SansFontName,
        FontSize = 16,
        Bold = true,
    };

    public Font TickFont { get; set; } = new();

    public abstract Edge Edge { get; }
    public bool ShowDebugInformation { get; set; } = false;

    public float MajorTickLength { get; set; } = 4;
    public float MajorTickLineWidth { get; set; } = 1;
    public Color MajorTickColor { get; set; } = Colors.Black;
    public TickStyle MajorTickStyle => new()
    {
        Length = MajorTickLength,
        LineWidth = MajorTickLineWidth,
        Color = MajorTickColor
    };

    public float MinorTickLength { get; set; } = 2;
    public float MinorTickLineWidth { get; set; } = 1;
    public Color MinorTickColor { get; set; } = Colors.Black;
    public TickStyle MinorTickStyle => new()
    {
        Length = MinorTickLength,
        LineWidth = MinorTickLineWidth,
        Color = MinorTickColor
    };

    public float FrameLineWidth { get; set; } = 1;
    public Color FrameColor { get; set; } = Colors.Black;

    public float Measure()
    {
        float largestTickSize = MeasureTicks();
        float largestTickLabelSize = Label.Measure().Height;
        float spaceBetweenTicksAndAxisLabel = 15;
        return largestTickSize + largestTickLabelSize + spaceBetweenTicksAndAxisLabel;
    }

    private float MeasureTicks()
    {
        using SKPaint paint = new()
        {
            Typeface = TickFont.ToSKTypeface(),
        };

        float largestTickHeight = 0;

        foreach (Tick tick in TickGenerator.GetVisibleTicks(Range))
        {
            PixelSize tickLabelSize = Drawing.MeasureString(tick.Label, paint);
            largestTickHeight = Math.Max(largestTickHeight, tickLabelSize.Height + 10);
        }

        return largestTickHeight;
    }

    public float GetPixel(double position, PixelRect dataArea)
    {
        double pxPerUnit = dataArea.Width / Width;
        double unitsFromLeftEdge = position - Min;
        float pxFromEdge = (float)(unitsFromLeftEdge * pxPerUnit);
        return dataArea.Left + pxFromEdge;
    }

    public double GetCoordinate(float pixel, PixelRect dataArea)
    {
        double pxPerUnit = dataArea.Width / Width;
        float pxFromLeftEdge = pixel - dataArea.Left;
        double unitsFromEdge = pxFromLeftEdge / pxPerUnit;
        return Min + unitsFromEdge;
    }

    private PixelRect GetPanelRectangleBottom(PixelRect dataRect, float size, float offset)
    {
        return new PixelRect(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Bottom + offset + size,
            top: dataRect.Bottom + offset);
    }

    private PixelRect GetPanelRectangleTop(PixelRect dataRect, float size, float offset)
    {
        return new PixelRect(
            left: dataRect.Left,
            right: dataRect.Right,
            bottom: dataRect.Top - offset,
            top: dataRect.Top - offset - size);
    }

    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset)
    {
        return Edge == Edge.Bottom
            ? GetPanelRectangleBottom(dataRect, size, offset)
            : GetPanelRectangleTop(dataRect, size, offset);
    }

    public void Render(SKSurface surface, PixelRect dataRect, float size, float offset)
    {
        PixelRect panelRect = GetPanelRect(dataRect, size, offset);

        float textDistanceFromEdge = 10;
        Pixel labelPoint = new(panelRect.HorizontalCenter, panelRect.Bottom - textDistanceFromEdge);

        if (ShowDebugInformation)
        {
            Drawing.DrawDebugRectangle(surface.Canvas, panelRect, labelPoint, Label.Color);
        }

        Label.Alignment = Alignment.LowerCenter;
        Label.Rotation = 0;
        Label.Draw(surface.Canvas, labelPoint);

        using SKFont tickFont = new()
        {
            Typeface = TickFont.ToSKTypeface(),
        };
        IEnumerable<Tick> ticks = TickGenerator.GetVisibleTicks(Range);

        AxisRendering.DrawTicks(surface, tickFont, panelRect, ticks, this, MajorTickStyle, MinorTickStyle);
        AxisRendering.DrawFrame(surface, panelRect, Edge, FrameLineWidth, FrameColor);
    }

    public double GetPixelDistance(double distance, PixelRect dataArea)
    {
        return distance * dataArea.Width / Width;
    }

    public double GetCoordinateDistance(double distance, PixelRect dataArea)
    {
        return distance / (dataArea.Width / Width);
    }
}
