//objectTrackingTutorial.cpp

//Written by  Kyle Hounslow 2013

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software")
//, to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
//IN THE SOFTWARE.

#include <sstream>
#include <list>
#include <iterator>
#include <string>
#include <iostream>
#include <C:\Users\timle\opencv\build\include\opencv\highgui.h>
#include <C:\Users\timle\opencv\build\include\opencv\cv.hpp>
#include <vector>
#include <fstream>

using namespace cv;
//initial min and max HSV filter values.
//these will be changed using trackbars
/*int H_MIN = 0; //WHITE
int H_MAX = 64;
int S_MAX = 18*/

/*int H_MIN = 0; //RED
int H_MAX = 5;
int S_MIN = 105*/

/*int H_MIN = 2; //ORANGE
int H_MAX = 11;
int S_MIN = 251*/

/*int H_MIN = 21; //YELLOW
int H_MAX = 25;
int S_MIN = 105*/

//is used to adjust the value for the white color because I use a bad camera and the value keeps fluctuating
int MEANSIZEW = 40000;
int MEANSIZEWMin = 10000;

//the size in px of one unit of Lego
float REFUNIT = 0;

int H_MIN = 0; //BASE
int H_MAX = 256;
int S_MIN = 0;
int S_MAX = 256;
int V_MIN = 0;
int V_MAX = 256;
int H_MINW = 0; //WHITE
int H_MAXW = 256;
int S_MINW = 100;
int S_MAXW = 0;
int V_MINW = 0;
int V_MAXW = 256;
int areaW = 0;
int H_MINB = 103;//BLUE
int H_MAXB = 126;
int S_MINB = 0;
int S_MAXB = 256;
int V_MINB = 0;
int V_MAXB = 256;
int areaB = 0;
int H_MINR = 0;//RED
int H_MAXR = 4;
int S_MINR = 105;
int S_MAXR = 256;
int V_MINR = 0;
int V_MAXR = 256;
int areaR = 0;
int H_MINY = 14;//YELLOW
int H_MAXY = 25;
int S_MINY = 132;
int S_MAXY = 256;
int V_MINY = 0;
int V_MAXY = 256;
int areaY = 0;
int H_MINO = 5;//ORANGE
int H_MAXO = 14;
int S_MINO = 223;
int S_MAXO = 256;
int V_MINO = 0;
int V_MAXO = 256;
int areaO = 0;

//path to file in which save the info
String PATH = "C:\\Users\\timle\\Desktop\\Stage2018\\LegoCalendar\\info.js";

//default capture width and height
const int FRAME_WIDTH = 640;
const int FRAME_HEIGHT = 480;
//max number of objects to be detected in frame
const int MAX_NUM_OBJECTS = 50;
//minimum and maximum object area
const int MIN_OBJECT_AREA = 100 * 100;
const int MAX_OBJECT_AREA = FRAME_HEIGHT*FRAME_WIDTH / 1.5;
//names that will appear at the top of each window
const String windowName = "Original Image";
const String windowName1 = "HSV Image";
const String windowName2 = "Thresholded Image";
const String windowName3 = "After Morphological Operations";
const String trackbarWindowName = "Trackbars";

typedef struct info {
	int x;
	int size;
	int color;
}info;

std::list <info> infList;

void on_trackbar(int, void*)
{//This function gets called whenever a
 // trackbar position is changed





}

String intToString(int number) {
	std::stringstream ss;
	ss << number;
	return ss.str();
}

String doubleToString(double number) {
	std::ostringstream strs;
	strs << number;
	return strs.str();
}


void createTrackbars() {
	//create window for trackbars


	namedWindow(trackbarWindowName, 0);
	//create memory to store trackbar name on window
	char TrackbarName[50];
	sprintf(TrackbarName, "H_MIN", H_MIN);
	sprintf(TrackbarName, "H_MAX", H_MAX);
	sprintf(TrackbarName, "S_MIN", S_MIN);
	sprintf(TrackbarName, "S_MAX", S_MAX);
	sprintf(TrackbarName, "V_MIN", V_MIN);
	sprintf(TrackbarName, "V_MAX", V_MAX);
	//create trackbars and insert them into window
	//3 parameters are: the address of the variable that is changing when the trackbar is moved(eg.H_LOW),
	//the max value the trackbar can move (eg. H_HIGH), 
	//and the function that is called whenever the trackbar is moved(eg. on_trackbar)
	//                                  ---->    ---->     ---->      
	createTrackbar("H_MIN", trackbarWindowName, &H_MINW, H_MAX, on_trackbar);
	createTrackbar("H_MAX", trackbarWindowName, &H_MAXW, H_MAX, on_trackbar);
	createTrackbar("S_MIN", trackbarWindowName, &S_MINW, S_MAX, on_trackbar);
	createTrackbar("S_MAX", trackbarWindowName, &S_MAXW, S_MAX, on_trackbar);
	createTrackbar("V_MIN", trackbarWindowName, &V_MINW, V_MAX, on_trackbar);
	createTrackbar("V_MAX", trackbarWindowName, &V_MAXW, V_MAX, on_trackbar);
}

//Saves the info in the list of info
void saveInfo(int x, int size, int color) {
	info inf;
	inf.x = x;
	inf.size = size;
	inf.color = color;
	for (std::list<info>::iterator it = infList.begin(); it != infList.end(); ++it) {
		info currentInf = *it;
		if (currentInf.color == inf.color) {
			infList.erase(it);
			break;
		}
	}
	infList.push_back(inf);
}

//displays the size of each block of lego detected
void displaySize(int x, int y, Mat &frame, Scalar color, std::vector<RotatedRect> vect, int ctsize, int colorId) {
	float key = 0;
	float comp;
	for (int i = 0; i < ctsize; i++)
	{
		Point2f rect_points[4]; vect[i].points(rect_points);
		for (int j = 0; j < 4; j++) {
			//Measures the distance between 2 points of the rectangle in order to keep the biggest one
			comp = sqrt(pow(rect_points[j].x - rect_points[(j + 1) % 4].x, 2) + pow(rect_points[j].y - rect_points[(j + 1) % 4].y, 2));
			if (comp > key) {
				key = comp;
			}
		}
	}
	int legosize = (int)round(key / REFUNIT);
	putText(frame, "Size " + intToString(legosize), Point(x, y + 75), 1, 1, color, 2);
	saveInfo(x, legosize, colorId);
}


//Comparator for the sort function
bool comp(info& first, info&second) {
	if (first.x < second.x) {
		return true;
	}
	return false;
}

//prints the infos in the txt in order to be read by the javascript program
void infoToTxt() {
	//Sorts the list in order to have the far right lego info printed first in the txt
	infList.sort(comp);
	std::ofstream myfile;
	myfile.open(PATH);
	myfile << "var str = \"";
	for (std::list<info>::iterator it = infList.begin(); it != infList.end(); ++it) {
		info currentInf = *it;
		myfile << intToString(currentInf.size) + ' ' + intToString(currentInf.color) + "-";
	}
	myfile << "\";";
	myfile.close();
}


int drawObject(int x, int y, Mat &frame, int hmn, int hmx, double area) {

	//use some of the openCV drawing functions to draw crosshairs
	//on your tracked image
	circle(frame, Point(x, y), 20, Scalar(0, 255, 0), 2);
	if (y - 25 > 0)
		line(frame, Point(x, y), Point(x, y - 25), Scalar(0, 255, 0), 2);
	else line(frame, Point(x, y), Point(x, 0), Scalar(0, 255, 0), 2);
	if (y + 25 < FRAME_HEIGHT)
		line(frame, Point(x, y), Point(x, y + 25), Scalar(0, 255, 0), 2);
	else line(frame, Point(x, y), Point(x, FRAME_HEIGHT), Scalar(0, 255, 0), 2);
	if (x - 25 > 0)
		line(frame, Point(x, y), Point(x - 25, y), Scalar(0, 255, 0), 2);
	else line(frame, Point(x, y), Point(0, y), Scalar(0, 255, 0), 2);
	if (x + 25 < FRAME_WIDTH)
		line(frame, Point(x, y), Point(x + 25, y), Scalar(0, 255, 0), 2);
	else line(frame, Point(x, y), Point(FRAME_WIDTH, y), Scalar(0, 255, 0), 2);

	putText(frame, intToString(x) + "," + intToString(y), Point(x, y + 30), 1, 1, Scalar(0, 255, 0), 2);

	//displays the color identified according to the hue valor as well as the number of pixels of the detected object
	if (hmn > 4 && hmx < 15) {
		putText(frame, "Orange " + doubleToString(area) + "px", Point(x, y + 50), 1, 1, Scalar(0, 140, 255), 2);
		putText(frame, "Orange " + doubleToString(area), Point(500, 20), 1, 1, Scalar(0, 140, 255), 2);
		return 6;
	}
	else if (hmn >= 0 && hmx < 5) {
		putText(frame, "Red " + doubleToString(area) + "px", Point(x, y + 50), 1, 1, Scalar(0, 0, 255), 2);
		putText(frame, "Red " + doubleToString(area), Point(500, 40), 1, 1, Scalar(0, 0, 255), 2);
		return 11;
	}
	else if (hmn > 13 && hmx < 26) {
		putText(frame, "Yellow " + doubleToString(area) + "px", Point(x, y + 50), 1, 1, Scalar(0, 255, 255), 2);
		putText(frame, "Yellow " + doubleToString(area), Point(500, 60), 1, 1, Scalar(0, 255, 255), 2);
		return 5;
	}
	else if (hmn == 0 && hmx == 256) {
		/*if (area > MEANSIZEW)
			V_MINW += 10;
		else if (area < MEANSIZEWMin) {
			V_MIN -= 10;
		}*/
		putText(frame, "White " + doubleToString(area) + "px", Point(x, y + 50), 1, 1, Scalar(255, 255, 255), 2);
		putText(frame, "White " + doubleToString(area), Point(500, 80), 1, 1, Scalar(255, 255, 255), 2);
	}
	else if (hmn > 102 && hmx > 125) {
		putText(frame, "Blue " + doubleToString(area) + "px", Point(x, y + 50), 1, 1, Scalar(255, 0, 0), 2);
		putText(frame, "Blue " + doubleToString(area), Point(500, 100), 1, 1, Scalar(255, 0, 0), 2);
		return 9;
	}
}

void morphOps(Mat &thresh) {

	//create structuring element that will be used to "dilate" and "erode" image.
	//the element chosen here is a 3px by 3px rectangle

	Mat erodeElement = getStructuringElement(MORPH_RECT, Size(3, 3));
	//dilate with larger element so make sure object is nicely visible
	Mat dilateElement = getStructuringElement(MORPH_RECT, Size(8, 8));

	erode(thresh, thresh, erodeElement);
	erode(thresh, thresh, erodeElement);


	dilate(thresh, thresh, dilateElement);
	dilate(thresh, thresh, dilateElement);



}

void getReferenceSize(Mat &frame, int hmn, int hmx, std::vector<RotatedRect> vect, int ctsize) {
	if (hmn > 13 && hmx < 26) {
		float key = 0;
		float comp;
		for (int i = 0; i < ctsize; i++)
		{
			Point2f rect_points[4]; vect[i].points(rect_points);
			for (int j = 0; j < 4; j++) {
				//Measures the distance between 2 points of the rectangle in order to keep the biggest one
				comp = sqrt(pow(rect_points[j].x - rect_points[(j + 1) % 4].x, 2) + pow(rect_points[j].y - rect_points[(j + 1) % 4].y, 2));
				if (comp > key) {
					key = comp;
				}
			}
		}
		REFUNIT = key / 4;
		putText(frame, "1Unit: " + doubleToString(REFUNIT), Point(0, 430), 1, 1, Scalar(0, 0, 0), 2);
	}

}

std::list <Rect> buttonList;
bool dispInf = false;
void CallBackFunc(int event, int x, int y, int flags, void* userdata)
{
	if (event == CV_EVENT_LBUTTONDOWN)
	{	
		for (std::list<Rect>::iterator it = buttonList.begin(); it != buttonList.end(); ++it) {
			Rect current = *it;
			if (current.contains(Point(x, y))) { //if the click is in the clickable zone i.e the rotated rect (the lego piece)
				if (!dispInf) {
					dispInf = true;
					infList.clear();
				}
				else {
					dispInf = false;
				}

			}
		}
	}
}


void trackFilteredObject(int &x, int &y, Mat threshold, Mat &cameraFeed, int &hmn, int &hmx) {

	Mat temp;
	threshold.copyTo(temp);
	//these two vectors needed for output of findContours
	std::vector< std::vector<Point> > contours;
	std::vector<Vec4i> hierarchy;
	//find contours of filtered image using openCV findContours function
	findContours(temp, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE);
	//use moments method to find our filtered object
	double refArea = 0;
	bool objectFound = false;
	int colorId;
	double area;
	if (hierarchy.size() > 0) {
		int numObjects = hierarchy.size();
		//if number of objects greater than MAX_NUM_OBJECTS we have a noisy filter
		if (numObjects < MAX_NUM_OBJECTS) {
			for (int index = 0; index >= 0; index = hierarchy[index][0]) {

				Moments moment = moments((cv::Mat)contours[index]);
				area = moment.m00;

				//if the area is less than 100px by 100px then it is probably just noise
				//if the area is the same as the 3/2 of the image size, probably just a bad filter
				if (area > MIN_OBJECT_AREA && area < MAX_OBJECT_AREA) {
					x = moment.m10 / area;
					y = moment.m01 / area;
					objectFound = true;
					
				}
				else objectFound = false;
			}
			//let user know you found an object
			if (objectFound == true) {
				setMouseCallback(windowName, CallBackFunc, &cameraFeed);
				putText(cameraFeed, "Tracking Object", Point(0, 50), 2, 1, Scalar(0, 255, 0), 2);

				std::vector<RotatedRect> minRect((int)contours.size());
				for (int i = 0; i < contours.size(); i++)
				{
					minRect[i] = minAreaRect(Mat(contours[i]));
				}
				//Sets the size in px of one unit of lego
				getReferenceSize(cameraFeed, hmn, hmx, minRect, contours.size());
				//displays the size in lego units of every object tracked
				if (dispInf) {//draw object location on screen
					colorId = drawObject(x, y, cameraFeed, hmn, hmx, area);
					displaySize(x, y, cameraFeed, Scalar(0, 0, 0), minRect, contours.size(), colorId);
				}
				for (int i = 0; i < contours.size(); i++)
				{
					Scalar color = Scalar(0, 0, 0);
					// contour
					//drawContours(cameraFeed, contours, i, color, 1, 8, std::vector<Vec4i>(), 0, Point());
					// rotated rectangle
					Point2f rect_points[4]; minRect[i].points(rect_points);
					//sets the clickable zone
					Rect r = Rect(rect_points[0], rect_points[2]);
					buttonList.push_back(r);
					for (int j = 0; j < 4; j++)
						line(cameraFeed, rect_points[j], rect_points[(j + 1) % 4], color, 1, 8);
				}
			}

		}
		else putText(cameraFeed, "Too much noise, please adjust filter", Point(0, 50), 1, 2, Scalar(0, 0, 255), 2);
	}
}


int main(int argc, char* argv[])
{
	namedWindow(windowName);
	//some boolean variables for different functionality within this
	//program
	bool trackObjects = true;
	bool useMorphOps = true;
	//Matrix to store each frame of the webcam feed
	Mat cameraFeed;
	//matrix storage for HSV image
	Mat HSV;
	//matrix storage for binary threshold image
	Mat thresholdw;
	Mat thresholdb;
	Mat thresholdr;
	Mat thresholdy;
	Mat thresholdo;
	//x and y values for the location of the object
	int xw = 0, yw = 0;
	int xb = 0, yb = 0;
	int xr = 0, yr = 0;
	int xy = 0, yy = 0;
	int xo = 0, yo = 0;
	//create slider bars for HSV filtering
	createTrackbars();
	//video capture object to acquire webcam feed
	VideoCapture capture;
	//open capture object at location zero (default location for webcam)
	capture.open(0);
	//set height and width of capture frame
	capture.set(CV_CAP_PROP_FRAME_WIDTH, FRAME_WIDTH);
	capture.set(CV_CAP_PROP_FRAME_HEIGHT, FRAME_HEIGHT);
	//start an infinite loop where webcam feed is copied to cameraFeed matrix
	//all of our operations will be performed within this loop
	while (1) {
		buttonList.clear();
		//store image to matrix
		capture.read(cameraFeed);
		//convert frame from BGR to HSV colorspace
		cvtColor(cameraFeed, HSV, COLOR_BGR2HSV);
		//filter HSV image between values and store filtered image to
		//threshold matrix
		inRange(HSV, Scalar(H_MINW, S_MINW, V_MINW), Scalar(H_MAXW, S_MAXW, V_MAXW), thresholdw);
		inRange(HSV, Scalar(H_MINB, S_MINB, V_MINB), Scalar(H_MAXB, S_MAXB, V_MAXB), thresholdb);
		inRange(HSV, Scalar(H_MINR, S_MINR, V_MINR), Scalar(H_MAXR, S_MAXR, V_MAXR), thresholdr);
		inRange(HSV, Scalar(H_MINY, S_MINY, V_MINY), Scalar(H_MAXY, S_MAXY, V_MAXY), thresholdy);
		inRange(HSV, Scalar(H_MINO, S_MINO, V_MINO), Scalar(H_MAXO, S_MAXO, V_MAXO), thresholdo);
		//perform morphological operations on thresholded image to eliminate noise
		//and emphasize the filtered object(s)
		if (useMorphOps) {
			morphOps(thresholdw);
			morphOps(thresholdb);
			morphOps(thresholdr);
			morphOps(thresholdy);
			morphOps(thresholdo);
		}

		//pass in thresholded frame to our object tracking function
		//this function will return the x and y coordinates of the
		//filtered object
		if (trackObjects) {
			trackFilteredObject(xw, yw, thresholdw, cameraFeed, H_MINW, H_MAXW);
			trackFilteredObject(xb, yb, thresholdb, cameraFeed, H_MINB, H_MAXB);
			trackFilteredObject(xr, yr, thresholdr, cameraFeed, H_MINR, H_MAXR);
			trackFilteredObject(xy, yy, thresholdy, cameraFeed, H_MINY, H_MAXY);
			trackFilteredObject(xo, yo, thresholdo, cameraFeed, H_MINO, H_MAXO);
		}

		//show frames 
		//imshow(windowName2, thresholdw);
		imshow(windowName, cameraFeed);
		//imshow(windowName1, HSV);


		//writing the infos in the info.js file
		infoToTxt();

		//delay 30ms so that screen can refresh.
		//image will not appear without this waitKey() command
		waitKey(10);
	}
	return 0;
}
