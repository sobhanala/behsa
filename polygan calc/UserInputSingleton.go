package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"
	"sync"
)

type UserInputSingleton struct{}

var instance *UserInputSingleton
var once sync.Once

func GetInstance() *UserInputSingleton {
	once.Do(func() {
		instance = &UserInputSingleton{}
	})
	return instance
}

func (u *UserInputSingleton) GetPointsFromUser() []Point {
	reader := bufio.NewReader(os.Stdin)
	fmt.Println("Enter the number of points:")
	nStr, _ := reader.ReadString('\n')
	n, _ := strconv.Atoi(strings.TrimSpace(nStr))

	var points []Point
	for i := 0; i < n; i++ {
		fmt.Printf("Enter x and y coordinates of point %d (space-separated):\n", i+1)
		line, _ := reader.ReadString('\n')
		xy := strings.Fields(line)
		x, _ := strconv.ParseFloat(xy[0], 64)
		y, _ := strconv.ParseFloat(xy[1], 64)
		points = append(points, NewPoint(x, y))
	}

	return points
}
