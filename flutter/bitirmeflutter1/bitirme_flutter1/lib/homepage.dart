import 'package:flutter/material.dart';
import 'services/sensor_services.dart';

class HomePage extends StatefulWidget {
  final Map<String, dynamic>? user;

  const HomePage({Key? key, this.user}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late final SensorService sensorService;
  List<dynamic> sensorData = [];
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    sensorService = SensorService('http://192.168.31.100:5273'); // Replace with your API base URL
    fetchSensorData();
  }

  Future<void> fetchSensorData() async {
    setState(() {
      isLoading = true;
    });

    final userId = widget.user?['id'];
    if (userId == null) return;

    try {
      final data = await sensorService.fetchSensorDataByUserId(userId.toString());
      setState(() {
        sensorData = data;
        isLoading = false;
      });
    } catch (error) {
      debugPrint('Error fetching sensor data: $error');
      setState(() {
        isLoading = false;
      });
    }
  }


  Future<void> addSensor(String sensorSerialNumber) async {
    final userId = widget.user?['id'];
    if (userId == null) return;

    try {
      await sensorService.addSensorData(
        sensorSerialNumber: sensorSerialNumber,
        userId: userId.toString(),
      );
      fetchSensorData(); // Reload the data after adding
    } catch (error) {
      debugPrint('Error adding sensor: $error');
    }
  }

  void showAddSensorDialog() {
    final controller = TextEditingController();

    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text('Add New Sensor'),
          content: TextField(
            controller: controller,
            decoration: const InputDecoration(
              labelText: 'Sensor Serial Number',
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.pop(context);
              },
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: () {
                final serialNumber = controller.text.trim();
                if (serialNumber.isNotEmpty) {
                  addSensor(serialNumber);
                  Navigator.pop(context);
                }
              },
              child: const Text('Confirm'),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('My Sensors')),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : sensorData.isEmpty
          ? const Center(child: Text('No sensors found. Add a new sensor to get started.'))
          : ListView.builder(
        itemCount: sensorData.length,
        itemBuilder: (context, index) {
          final sensor = sensorData[index];
          final sensorSerial = sensor['sensorSerialNumber'] ?? 'Unknown';
          final plantId = sensor['plantId'];

          return Card(
            margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
            child: ListTile(
              title: Text('Sensor: $sensorSerial'),
              trailing: plantId == null
                  ? ElevatedButton.icon(
                icon: const Icon(Icons.camera_alt),
                label: const Text('Identify Your Plant'),
                onPressed: () {
                  // Handle plant identification here
                  debugPrint('Identify plant for sensor $sensorSerial');
                },
              )
                  : Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text('Plant ID: $plantId'),
                  const SizedBox(width: 8),
                  DropdownButton<String>(
                    items: const [
                      DropdownMenuItem(
                        value: 'Action1',
                        child: Text('Action 1'),
                      ),
                      DropdownMenuItem(
                        value: 'Action2',
                        child: Text('Action 2'),
                      ),
                    ],
                    onChanged: (value) {
                      // Handle dropdown actions here
                      debugPrint('Dropdown action: $value');
                    },
                    hint: const Text('Actions'),
                  ),
                ],
              ),
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: showAddSensorDialog,
        child: const Icon(Icons.add),
      ),
    );
  }
}
