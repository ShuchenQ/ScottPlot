﻿namespace ScottPlot.DataSources;

/// <summary>
/// This data source manages X/Y points as a collection of coordinates
/// </summary>
public class ScatterSourceCoordinatesArray(Coordinates[] coordinates) : IScatterSource
{
    private readonly Coordinates[] Coordinates = coordinates;

    public int MinRenderIndex { get; set; } = 0;
    public int MaxRenderIndex { get; set; } = int.MaxValue;
    private int RenderIndexCount => Math.Min(Coordinates.Length - 1, MaxRenderIndex) - MinRenderIndex + 1;

    public IReadOnlyList<Coordinates> GetScatterPoints()
    {
        return Coordinates
            .Skip(MinRenderIndex)
            .Take(RenderIndexCount)
            .ToList();
    }

    public AxisLimits GetLimits()
    {
        ExpandingAxisLimits limits = new();
        limits.Expand(Coordinates.Skip(MinRenderIndex).Take(RenderIndexCount));
        return limits.AxisLimits;
    }

    public CoordinateRange GetLimitsX()
    {
        return GetLimits().Rect.XRange;
    }

    public CoordinateRange GetLimitsY()
    {
        return GetLimits().Rect.YRange;
    }

    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15)
    {
        double maxDistanceSquared = maxDistance * maxDistance;
        double closestDistanceSquared = double.PositiveInfinity;

        int closestIndex = 0;
        double closestX = double.PositiveInfinity;
        double closestY = double.PositiveInfinity;

        for (int i2 = 0; i2 < RenderIndexCount; i2++)
        {
            int i = MinRenderIndex + i2;
            double dX = (Coordinates[i].X - mouseLocation.X) * renderInfo.PxPerUnitX;
            double dY = (Coordinates[i].Y - mouseLocation.Y) * renderInfo.PxPerUnitY;
            double distanceSquared = dX * dX + dY * dY;

            if (distanceSquared <= closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closestX = Coordinates[i].X;
                closestY = Coordinates[i].Y;
                closestIndex = i;
            }
        }

        return closestDistanceSquared <= maxDistanceSquared
            ? new DataPoint(closestX, closestY, closestIndex)
            : DataPoint.None;
    }
}
