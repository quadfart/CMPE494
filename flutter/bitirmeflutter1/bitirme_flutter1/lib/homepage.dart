import 'package:bitirme_flutter1/plantDetailsScreen.dart';
import 'package:bitirme_flutter1/profile.dart';
import 'package:flutter/material.dart';
import 'model_test_screen.dart';
import 'services/sensor_services.dart';

class HomePage extends StatefulWidget {
  final Map<String, dynamic> user;

  const HomePage({Key? key, required this.user}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late final SensorService sensorService;
  List<dynamic> sensorData = [];
  bool isLoading = true;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    sensorService = SensorService(
        'http://ec2-13-53-214-45.eu-north-1.compute.amazonaws.com:80'); // Replace with your API base URL
    fetchSensorData();
  }

  Future<void> fetchSensorData() async {
    setState(() {
      isLoading = true;
    });

    final userId = widget.user?['id'];
    if (userId == null) return;

    try {
      final data =
          await sensorService.fetchSensorDataByUserId(userId.toString());
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

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
    if (index == 1) {
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => ProfilePage(
            user: widget.user,
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    Size size = MediaQuery.of(context).size;
    return Scaffold(
      appBar: AppBar(
        flexibleSpace: SafeArea(
          child: Container(
            padding: EdgeInsets.only(
              top: MediaQuery.of(context).padding.top,
              left: size.width * 0.05,
            ),
            child: const Text(
              'My Plants',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 20,
              ),
            ),
          ),
        ),
        title: Image.asset(
          'assets/images/logo.png', // Logo file path
          height: 50, // Adjusted height for better visibility
        ),
        centerTitle: true,
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : sensorData.isEmpty
              ? const Center(
                  child: Text(
                      'No sensors found. Add a new sensor to get started.'))
              : Column(
                  children: [
                    const SizedBox(height: 16),
                    Center(
                      child: ElevatedButton.icon(
                        onPressed: () {
                          fetchSensorData();
                        },
                        label: const Text("Update Sensor Data"),
                        icon: const Icon(Icons.update, color: Colors.white),
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 20,
                            vertical: 15,
                          ),
                          backgroundColor: Colors.green,
                          foregroundColor: Colors.white,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(10),
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    Expanded(
                      child: ListView.builder(
                        itemCount: sensorData.length,
                        itemBuilder: (context, index) {
                          final sensor = sensorData[index];
                          final sensorSerial =
                              sensor['sensorSerialNumber'] ?? 'Unknown';
                          final plantId = sensor['plantId'];
                          final id = sensor["id"];
                          
                          return GestureDetector(
                            onTap: () {
                              if (plantId != null) {
                                Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (context) => PlantDetailsScreen(
                                      plantId: plantId,
                                      id: id,
                                      sensor: sensor,
                                    ),
                                  ),
                                );
                              }
                            },
                            child: Card(
                              margin: const EdgeInsets.symmetric(
                                  vertical: 8, horizontal: 16),
                              child: ListTile(
                                title: Text('Sensor: $sensorSerial'),
                                trailing: plantId == null
                                    ? ElevatedButton.icon(
                                        icon: const Icon(
                                          Icons.camera_alt,
                                          color: Colors
                                              .white, // Changed icon color to white
                                        ),
                                        label:
                                            const Text('Identify Your Plant'),
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor:
                                              Colors.green[800], // Dark green
                                          foregroundColor: Colors.white,
                                        ),
                                        onPressed: () {
                                          Navigator.push(
                                            context,
                                            MaterialPageRoute(
                                              builder: (context) =>
                                                  ImageClassifierScreen(
                                                id: id,
                                              ),
                                            ),
                                          );
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
                                              debugPrint(
                                                  'Dropdown action: $value');
                                            },
                                            hint: const Text('Actions'),
                                          ),
                                        ],
                                      ),
                              ),
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                ),
      floatingActionButton: FloatingActionButton(
        onPressed: showAddSensorDialog,
        backgroundColor: Colors.green, // Changed to green
        child: const Icon(Icons.add),
      ),
      bottomNavigationBar: BottomNavigationBar(
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Home',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Profile',
          ),
        ],
        currentIndex: _selectedIndex,
        selectedItemColor: Colors.green[800], // Dark green for selected item
        onTap: _onItemTapped,
      ),
    );
  }
}
