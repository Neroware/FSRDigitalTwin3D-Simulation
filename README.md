# FSRDigitalTwin3D-Simulation
FSRDigitalTwin3D is a process-oriented simulation environment with semantic reasoning for use cases of social human-robot interaction/collaboration. It consists of a knowledge server to provide semantic reasoning and connected infrastructure (digital layer), and a [Unity simulation tool](https://github.com/Neroware/FSRDigitalTwin3D-Simulation) (virtual layer) to simulate social human-robot collaboration in an interactive 3D-environment using an event-discrete and process-oriented approach.

The design philosophy is to provide a "playground" for virtual robotics simulations abiding to the standards of modern Industry 4.0 applications. Therefore, for external communication, the [AASv3.0/REST](https://industrialdigitaltwin.org/content-hub/standardisierter-digitaler-zwilling-reif-fuer-die-industrie-6208) standard is deployed. The server additionally features a AASv3.0/gRPC API.

**Note:** This repository is the Unity simulation client. To run simulation scenarios, you also require the [knowledge server](https://github.com/Neroware/FSRDigitalTwin3D) as an additional digitization layer. The server repo has a submodule link to this repository.

*We believe, our digital twin wins the buzzword-bingo of 'Semantic Data Twin' and 'Process Data Twin'...*

## Requirements
This project has OS support for Linux and Windows.

- .NET 8.*
- Unity 2022.3.8f1

## Installation
### Unity Simulation Client (Virtualization Layer)
The client is located at ```/FSR/DigitalTwin/Client/Unity/FSR.DigitalTwin.Client.Unity/```:

1. Navigate to folder ```/FSR/DigitalTwin/Client/Unity/```
2. Load Git submodule ```git submodule update --init --recursive .```
3. Navigate into Unity project ```cd FSR.DigitalTwin.Client.Unity/```
4. Run script ```install-client-plugins```
5. Import the Unity project into Unity Hub and open

**Note**: This project is maintained by the [Chair of Digital Manufacturing](https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi/) of the University of Augsburg.

**Note**: If you have questions or suggestions that are not suitable for discussion within the issues section, feel free to send an e-mail to raoul.zebisch@uni-a.de.

**Note:** This digital twin is developed as a sub-project for the research association FORSocialRobots, namely "Subproject 4: Simulation and validation of socially cognitive robots in the digital twin"

**Licence**: MIT