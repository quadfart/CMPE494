#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <DHT.h>

// Wi-Fi Ayarları
const char* ssid = "TurkTelekom_T9C79";
const char* password = "4DLjabXN";

// Kullanıcı Tarafından Belirlenen Sunucu IP'si
String serverIP = "ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80";
bool useDynamicIP = false;

// DHT Ayarları
#define DHTPIN 26     
#define DHTTYPE DHT11 
DHT dht(DHTPIN, DHTTYPE); 

// Toprak Nemi Sensörü
#define SOIL_MOISTURE_PIN 34 
const int DRY_VALUE = 4095;  // Kuru toprak (maksimum direnç)
const int WET_VALUE = 1200; 

// Motor Sürücü Pinleri 
#define MOTOR_IN1 32
#define MOTOR_IN2 33
#define MOTOR_ENA 25 

String sensorSerialNumber = "group4-13"; 

// Sulama Bilgisi
int irrigationAmount = 0; //(ml)
int wateringFrequency = 0; 
unsigned long irrigationInterval = 0; // Sulama aralığı (milisaniye)
unsigned long lastIrrigationTime = 0; // Son sulama zamanı


String getBaseURL() {
    return "http://" + serverIP + "/api";

}


void performIrrigation(int amount) {
  if (amount > 0) {
    Serial.println("Irrigation started!");
    Serial.print(amount);
    Serial.println(" ml irrigation in progress...");

    // Sulama miktarına göre çalışma süresini hesapla
    unsigned long operationTime = amount * 80; // 1 ml için 80 ms

    digitalWrite(MOTOR_IN1, HIGH);
    digitalWrite(MOTOR_IN2, LOW);
    analogWrite(MOTOR_ENA, 100);

    delay(operationTime); // Çalışma süresine göre bekle

    digitalWrite(MOTOR_IN1, LOW);
    digitalWrite(MOTOR_IN2, LOW);
    analogWrite(MOTOR_ENA, 0);

    Serial.println("Irrigation completed!");
    lastIrrigationTime = millis(); // update irrigation time
  } else {
    Serial.println("Invalid irrigation amount, no operation performed.");
  }
}

// Fonksiyon: Sunucudan Sulama Bilgilerini Al
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
           irrigationInterval = 7UL * 24UL * 60UL * 60UL * 1000UL; // one week 
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

// Send sensor data 
void sendData(int soilMoisture, int airHumidity, int temperature, String sensorSerialNumber) {
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
  delay(10000);
  dht.begin();

  WiFi.begin(ssid, password);
  delay(500);
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

  pinMode(SOIL_MOISTURE_PIN, INPUT);

  fetchIrrigationInfo(sensorSerialNumber);
  performIrrigation(irrigationAmount);
}

void loop() {
 int soilValue = analogRead(SOIL_MOISTURE_PIN);
  int soilMoisturePercentage = map(soilValue, DRY_VALUE, WET_VALUE, 0, 100);
  soilMoisturePercentage = constrain(soilMoisturePercentage, 0, 100); // %0-%100 aralığına sınırla
  int temperature = dht.readTemperature();
  int humidity = dht.readHumidity();

  if (isnan(temperature) || isnan(humidity)) {
    Serial.println("DHT11 Data Unavailable! Check connections.");
  } else {
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.print(" °C    Humidity: ");
    Serial.print(humidity);
    Serial.println(" %");
    Serial.print("Ham Toprak Nem Değeri: ");
    Serial.println(soilValue);

    Serial.print("Soil Moisture Percentage (%): ");
    Serial.print(soilMoisturePercentage);
    Serial.println(" %");

    sendData(soilMoisturePercentage, humidity, temperature, sensorSerialNumber);


  delay(60000); 
}
}
