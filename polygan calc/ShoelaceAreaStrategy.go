package main

import "math"

type ShoelaceAreaStrategy struct{}

func (s *ShoelaceAreaStrategy) CalculateArea(polygon Polygon) float64 {
	points := polygon.Points
	n := len(points)
	area := 0.0

	for i := 0; i < n; i++ {
		p1 := points[i]
		p2 := points[(i+1)%n]
		area += p1.X*p2.Y - p1.Y*p2.X
	}

	return math.Abs(area) / 2.0
}
