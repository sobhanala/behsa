package main

type Polygon struct {
	Points []Point
}

type PolygonBuilder struct {
	points []Point
}

func NewPolygonBuilder(points []Point) *PolygonBuilder {
	return &PolygonBuilder{points: points}
}

func (b *PolygonBuilder) Build() Polygon {
	return Polygon{Points: b.points}
}
