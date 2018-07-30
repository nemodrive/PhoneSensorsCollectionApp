# PhoneSensorsCollectionApp

The application is desined to collect data from the sensors of an Android Smartphone: GPS, accelerometer, gyroscope, magnetometer.
The Android application was developed using Unity. It uses websockets to send the data to a PC or other device for storage.
Every message is a JSON string that contains the information gathered from all sensors during a frame together with a timestamp.
The repo contanins also server implementations in Python 2.7 and Python 3.6. 
In order to run the application and the server, an IP address and a Port number must be specified.
