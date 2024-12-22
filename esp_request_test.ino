#include <WiFi.h>
#include <HTTPClient.h>

const char* ssid = "Xiaomi_91AB"; // Replace with your WiFi SSID
const char* password = "b3c8f51A"; // Replace with your WiFi password

const String apiUrl  = "http://192.168.31.100:5273/api/SensorDataLog/AddSensorDataLog"; // Replace with your API URL

void setup() {
  // Start the Serial communication
  Serial.begin(115200);

  // Connect to Wi-Fi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }

  Serial.println("Connected to WiFi");
}

void loop() {
  // Continuously send data
  sendData();
  
  // Wait for 10 seconds before sending the next data (adjust as needed)
  delay(10000); // 10 seconds
}

void sendData() {
  Serial.print("ESP32 IP Address: ");
  Serial.println(WiFi.localIP());

  // Create an HTTPClient object
  HTTPClient http;

  // Start the HTTP request
  http.begin(apiUrl);

  // Set the timeout for the HTTP request (5000 milliseconds = 5 seconds)
  http.setTimeout(5000);

  // Specify content type
  http.addHeader("Content-Type", "application/json");

  // Create a JSON body with dummy data
  String jsonData = "{\"sensorSerialNumber\": \"ESP32-001\", \"temperature\": 32, \"moisture\": 32, \"soilMoisture\": 32}";

  // Send the POST request with the JSON data
  int httpResponseCode = http.POST(jsonData);

  // Check the response
  if (httpResponseCode > 0) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);

    // If the response code is 400, try printing the response body to get more details
    if (httpResponseCode == 400) {
      String responseBody = http.getString();
      Serial.print("Response Body: ");
      Serial.println(responseBody);
    }
  } else {
    Serial.print("Error sending POST request. Error code: ");
    Serial.println(httpResponseCode);
  }

  // End the HTTP request
  http.end();
}
