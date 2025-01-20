#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <DHT.h>

// Wi-Fi Ayarları
const char* ssid = "Selin iPhone’u";
const char* password = "selin123";

// Sunucu 
String serverIP = "ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80";
bool useDynamicIP = false; // Eğer dinamik IP kullanılacaksa bunu true yapın

// DHT settings
#define DHTPIN 26     
#define DHTTYPE DHT11 
DHT dht(DHTPIN, DHTTYPE);

// soil moisture settings
#define SOIL_MOISTURE_PIN 14 


#define MOTOR_IN1 32
#define MOTOR_IN2 33
#define MOTOR_ENA 25 

String sensorSerialNumber = "qwe"; 


int irrigationAmount = 0; // (ml)
int wateringFrequency = 0; // weekly irrigation
unsigned long irrigationInterval = 0; // miliseconds
unsigned long lastIrrigationTime = 0; 

// Base URL 
String getBaseURL() {
    return "http://" + serverIP + "/api";
}


void performIrrigation(int amount) {
  if (amount > 0) {
    Serial.println("Irrigation started!");
    Serial.print(amount);
    Serial.println(" ml irrigation in progress...");

    // Calculate working time according to irrigation amount
    unsigned long operationTime = amount * 80; // 1 ml için 80 ms

    digitalWrite(MOTOR_IN1, HIGH);
    digitalWrite(MOTOR_IN2, LOW);
    analogWrite(MOTOR_ENA, 100);

    delay(operationTime); //wait

    digitalWrite(MOTOR_IN1, LOW);
    digitalWrite(MOTOR_IN2, LOW);
    analogWrite(MOTOR_ENA, 0);

    Serial.println("Irrigation completed!");
    lastIrrigationTime = millis(); // update irrigation time 
  } else {
    Serial.println("Invalid irrigation amount, no operation performed.");
  }
}

void fetchIrrigationInfo(String sensorSerialNumber) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String serverNameGet = getBaseURL() + "/Plant/PlantBySensorSerialNumber/" + sensorSerialNumber;
    http.begin(serverNameGet);

    int httpResponseCode = http.GET();

    if (httpResponseCode > 0) {
      String payload = http.getString();
      Serial.println("Received Irrigation Info: " + payload);

      StaticJsonDocument<256> jsonDoc;
      DeserializationError error = deserializeJson(jsonDoc, payload);

      if (!error) {
        irrigationAmount = jsonDoc["irrigationAmount"];
        wateringFrequency = jsonDoc["wateringFrequency"];

        if (wateringFrequency == 1) {
           irrigationInterval = 7UL * 24UL * 60UL * 60UL * 1000UL; // weekly
        } else if (wateringFrequency == 2) {
           irrigationInterval = 3.5 * 24UL * 60UL * 60UL * 1000UL; // half week
        } else {
           irrigationInterval = 0; 
        }
        Serial.print("Irrigation Interval (ms): ");
        Serial.println(irrigationInterval);
      } else {
        Serial.println("Error parsing irrigation info JSON!");
      }
    } else {
      Serial.print("Failed to fetch irrigation info. HTTP Error: ");
      Serial.println(httpResponseCode);
    }

    http.end();
  } else {
    Serial.println("No Wi-Fi connection, unable to fetch irrigation info.");
  }
}

void veriGonder(int soilMoisture, int airHumidity, int temperature, String sensorSerialNumber) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String serverNamePost = getBaseURL() + "/SensorDataLog/AddSensorDataLog";
    http.begin(serverNamePost);

    StaticJsonDocument<1024> jsonDoc;
    jsonDoc["sensorSerialNumber"] = sensorSerialNumber;
    jsonDoc["temperature"] = temperature;
    jsonDoc["moisture"] = airHumidity;
    jsonDoc["soilMoisture"] = soilMoisture;

    String jsonData;
    serializeJson(jsonDoc, jsonData);
    Serial.println("Sent JSON data: " + jsonData);

    http.addHeader("Content-Type", "application/json");
    int httpResponseCode = http.POST(jsonData);

    if (httpResponseCode > 0) {
      Serial.print("Data successfully sent. HTTP Response Code: ");
      Serial.println(httpResponseCode);
    } else {
      Serial.print("Data send error:  ");
      Serial.println(httpResponseCode);
    }

    http.end();
  } else {
    Serial.println("No Wi-Fi connection, unable to send data.");
  }
}

void setup() {
  Serial.begin(115200);
  Serial.println("System Starting...");

  dht.begin();

  WiFi.begin(ssid, password);
  delay()
  Serial.print("Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWi-Fi connection successful");
  Serial.print("IP Adresi: ");
  Serial.println(WiFi.localIP());

  pinMode(MOTOR_IN1, OUTPUT);
  pinMode(MOTOR_IN2, OUTPUT);
  pinMode(MOTOR_ENA, OUTPUT);

  digitalWrite(MOTOR_IN1, LOW);
  digitalWrite(MOTOR_IN2, LOW);
  analogWrite(MOTOR_ENA, 0);

  fetchIrrigationInfo(sensorSerialNumber);
  performIrrigation(irrigationAmount);
}

void loop() {

   if (irrigationInterval > 0 && millis() - lastIrrigationTime >= irrigationInterval) {
    Serial.println("Irrigation conditions are OK. Starting irrigation..."");
    Serial.print("Irrigation Interval: ");
    Serial.println(irrigationInterval);
    Serial.print("Last Irrigation Time : ");
    Serial.println(lastIrrigationTime);
    Serial.print("millis() : ");
    Serial.println(millis());

    performIrrigation(irrigationAmount);
} else {
    Serial.println("Irrigation conditions are not OK.");
    Serial.print("Irrigation Interval: ");
    Serial.println(irrigationInterval);
    Serial.print("millis() - lastIrrigationTime: ");
    Serial.println(millis() - lastIrrigationTime);
}
  int temperature = dht.readTemperature();
  int humidity = dht.readHumidity();
  int soilMoisturePercentage = analogRead(SOIL_MOISTURE_PIN); 

  

  if (isnan(temperature) || isnan(humidity)) {
    Serial.println("DHT11 Data Unavailable! Check connections.");
  } else {
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.print(" °C    Humidity: ");
    Serial.print(humidity);
    Serial.println(" %");

    Serial.print("Toprak Nem Seviyesi (%): ");
    Serial.print(soilMoisturePercentage);
    Serial.println(" %");

    veriGonder(soilMoisturePercentage, humidity, temperature, sensorSerialNumber);


  delay(10000); // 10 saniyede bir döngü
}
}