Product : Bezier Curve Editor Package
Studio : Arkham Interactive
Date : September 9th, 2013
Version : 1.0
Email : support@arkhaminteractive.com

How to use:
	1) Add BezierCurve package to your Unity project
	2a) Add BezierCurve.cs script from Assets/BezierCurves/Scripts to any object
	2b) Alternatively, select GameObject/Create Other/Bezier Curve
	3) Use "Add Point" button to add bezier points to the curve
	4) Use "X" button to remove bezier points from the curve
	5) Use "/\" or "\/" to move points up or down in the curve order

 - The BezierCurve class also contains static functions used for getting points on first, second, and third order bezier curves. 
 - These functions take the positions of the anchor points as arguments.
 - Instances of the BezierCurve object use these same functions to calculate positions.