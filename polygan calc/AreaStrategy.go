package main

type AreaStrategy interface {
	CalculateArea(polygon Polygon) float64
}
