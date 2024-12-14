import 'package:flutter/material.dart';
import 'identify.ident.dart';

class ProcessScreen extends StatefulWidget {
  final String photoPath;

  const ProcessScreen({Key? key, required this.photoPath}) : super(key: key);

  @override
  _ProcessScreenState createState() => _ProcessScreenState();
}

class _ProcessScreenState extends State<ProcessScreen> {
  @override
  void initState() {
    super.initState();
    _navigateToIdentifyPlantScreen();
  }

  Future<void> _navigateToIdentifyPlantScreen() async {
    // 5 saniye bekler ve ardından IdentifyPlantScreen'e geçiş yapmak için bunu yazdık!!!
    await Future.delayed(const Duration(seconds: 5));
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
        builder: (context) => IdentifyPlantScreen(photoPath: widget.photoPath),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: const [
            CircularProgressIndicator(
              color: Colors.green,
            ),
            SizedBox(height: 20),
            Text(
              "Processing...", // İşleniyor mesajını gösteriyorum!
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Color.fromARGB(255, 63, 198, 10),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
