#include "DHT.h"

// DHT Sensör Ayarları
#define DHTPIN 26     
#define DHTTYPE DHT11 
DHT dht(DHTPIN, DHTTYPE);


#define SOIL_MOISTURE_PIN 14 

void setup() {
  Serial.begin(115200);
  Serial.println("DHT11 ve Toprak Nem Sensörü Başlatılıyor...");

  dht.begin(); 
}

void loop() {
  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

  if (isnan(temperature) || isnan(humidity)) {
    Serial.println("DHT11 Verileri Alınamıyor! Bağlantıları kontrol edin.");
  } else {
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.print(" °C    Humidity: ");
    Serial.print(humidity);
    Serial.println(" %");
  }

 
  int soilMoistureValue = analogRead(SOIL_MOISTURE_PIN); 
  float soilMoisturePercentage = map(soilMoistureValue, 4095, 1200, 0, 100);

  Serial.print("Toprak Nem Seviyesi (Ham Değer): ");
  Serial.print(soilMoistureValue);
  Serial.print("    Toprak Nem Seviyesi (%): ");
  Serial.print(soilMoisturePercentage);
  Serial.println(" %");

  delay(2000);
}