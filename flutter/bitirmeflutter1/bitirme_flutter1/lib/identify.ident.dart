import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'dart:io';
import 'success_screen.dart'; // Success ekranını import edin

class IdentifyPlantScreen extends StatefulWidget {
  final String photoPath;

  const IdentifyPlantScreen({super.key, required this.photoPath});

  @override
  _IdentifyPlantScreenState createState() => _IdentifyPlantScreenState();
}

class _IdentifyPlantScreenState extends State<IdentifyPlantScreen> {
  String plantName = "Mint"; // Geçici olarak sabit bitki adı
  DateTime? selectedDate;
  TimeOfDay? selectedTime;

  // Tarih seçme fonksiyonu
  Future<void> _selectDate(BuildContext context) async {
    final DateTime? pickedDate = await showDatePicker(
      context: context,
      initialDate: selectedDate ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
    );
    if (pickedDate != null) {
      setState(() {
        selectedDate = pickedDate;
      });
    }
  }

  // Saat seçme fonksiyonu
  Future<void> _selectTime(BuildContext context) async {
    final TimeOfDay? pickedTime = await showTimePicker(
      context: context,
      initialTime: selectedTime ?? TimeOfDay.now(),
    );
    if (pickedTime != null && pickedTime != selectedTime) {
      setState(() {
        selectedTime = pickedTime;
      });
    }
  }

  // Save butonu fonksiyonu
  void savePlant() {
    // Veriyi Navigator.pop ile geri döndür
    final plantData = {
      "plantName": plantName,
      "photoPath": widget.photoPath,
      "selectedDate": selectedDate?.toIso8601String(),
      "selectedTime": selectedTime?.format(context),
    };
    Navigator.pop(context, plantData); // Veriyi döndür

    // Success ekranına yönlendirme
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const SuccessScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    String formattedDate = selectedDate != null
        ? DateFormat('yyyy-MM-dd').format(selectedDate!)
        : "Select Date";
    return Scaffold(
      appBar: AppBar(
        title: const Text("Identify Plant"),
        backgroundColor: Colors.green,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              "Image Result",
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 10),
            Container(
              height: 200,
              width: double.infinity,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(15),
                image: DecorationImage(
                  image:
                      FileImage(File(widget.photoPath)), // Fotoğraf gösterimi
                  fit: BoxFit.cover,
                ),
              ),
            ),
            const SizedBox(height: 20),
            const Text(
              "Plant Name",
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 5),
            TextField(
              controller: TextEditingController(text: plantName),
              readOnly: true, // Kullanıcı tarafından değiştirilemez
              decoration: const InputDecoration(
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 20),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  formattedDate != null
                      ? "Date: $formattedDate"
                      : "Select Date",
                  style: const TextStyle(fontSize: 16),
                ),
                TextButton(
                  onPressed: () => _selectDate(context),
                  child: const Text("Select Date"),
                ),
              ],
            ),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  selectedTime != null
                      ? "Time: ${selectedTime!.format(context)}"
                      : "Select Time",
                  style: const TextStyle(fontSize: 16),
                ),
                TextButton(
                  onPressed: () => _selectTime(context),
                  child: const Text("Select Time"),
                ),
              ],
            ),
            const SizedBox(height: 30),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: savePlant,
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 15),
                  backgroundColor: Colors.green,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
                child: const Text(
                  "Save",
                  style: TextStyle(fontSize: 18, color: Colors.white),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
