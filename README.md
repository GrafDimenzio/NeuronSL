# NeuonSL

## Overview
NeuonSL is a project designed to enhance the functionality of the SCP Secret Laboratory (SCP: SL) server by integrating the Universal Mod Loader [Neuron](https://github.com/AnomalousCoders/Neuron). Neuron provides a powerful framework for loading plugins and modules while managing their dependencies, making it easier for developers to write APIs within modules that can be utilized by other modules and plugins. Additionally, Neuron includes its own Event System with prioritization and meta-reflection capabilities, allowing seamless interaction with the API through attributes within modules.

## Features
The NeuonSL project consists of the following core components:

### 1. Neuron Core
The core functionality of Neuron is responsible for loading plugins and modules, resolving dependencies, and providing a solid foundation for extending the SCP: SL server.

### 2. Patcher Module
The Patcher Module utilizes [Harmony](https://github.com/pardeike/Harmony) to enable the execution of patches solely through attributes. This module streamlines the process of making modifications to the game by allowing developers to easily apply changes through attributes without having to modify the game's original code.

### 3. Command Module
The Command Module offers an API for creating new command handlers and consoles quickly. It simplifies the process of implementing custom commands within the SCP: SL environment.

### 4. Config Module
The Config Module simplifies the registration and management of configurations through attributes. It automatically creates and updates configurations, while also providing support for translations in multiple languages.

In addition to the core components, NeuonSL includes the following custom modules:

### 5. Permission Module
The Permission Module implements the permission system from Synapse, providing comprehensive permission management capabilities within the SCP: SL server.

### 6. SlCommand Module
The SlCommand Module integrates the Neuron Command Module into the SCP: SL environment. It allows for seamless execution of commands within the game, leveraging the power of Neuron's command handling capabilities.

### 7. Reload Module
The Reload Module offers an API for reloading configurations, plugins, and modules. This module provides the flexibility to make changes on the fly without restarting the entire server.

## Installation and Usage
To install NeuonSL and Neuron for your SCP: SL server, follow these steps:

1. Download the latest release of NeuonSL from the repository's Releases page.
2. Locate the application data folder for your operating system:
    * Windows: Open the File Explorer and enter %appdata% in the search bar. Press Enter. This will take you to the AppData folder. Look for the SCP Secret Laboratory folder.
    * Linux: Open your file manager and navigate to the .config directory in your home folder. Inside, locate the SCP Secret Laboratory folder.
3. Copy the contents of the NeuonSL zip file into the SCP Secret Laboratory folder.
4. Start your SCP: SL server. NeuonSL and Neuron will automatically generate the necessary files and configurations.

## License
NeuonSL is released under the GNU General Public License v3.0. Please see the LICENSE file for more details.
