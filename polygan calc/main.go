package main

import "fmt"

func main() {
	userInput := GetInstance()
	points := userInput.GetPointsFromUser()

	polygon := NewPolygonBuilder(points).Build()

	areaCalculation := &ShoelaceAreaStrategy{}
	area := areaCalculation.CalculateArea(polygon)

	fmt.Printf("The area of the polygon is: %.2f\n", area)
}
