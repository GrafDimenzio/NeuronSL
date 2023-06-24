# NeuronSL
Neuron SL is a Bootstrap to run [Neuron](https://github.com/AnomalousCoders/Neuron) on a SCP: Secret Laboratory Game Server.

## What is Neuron?
Neuron itself doesn't change the game in anyway and it also doesn't provide a API for the Game itself.
Neuron itself is a universal modding framework which takes care of module and plugin loading as well as resolving dependencies between them.
This means the huge benefit of using Neuron instead the normal NW API is that it becomes way easier to create a own Custom API inside a Module and then to use it with other Modules and Plugins.

## Neuron Modules
Neuron already comes with 3 Modules which are handy for game modding:
* CommandsModule : This Module adds a API for implementing a Command System into any Game with ease
* PatcherModule : This Modules helps you to Patch any Game with [Harmony](https://github.com/pardeike/Harmony)
* ConfigModule: Thise Modules creates Configs and Translations for your Plugins and Modules

## Custom Modules
In this Repo are also 3 more Modules:
* ReloadModule : Adds a Reload Event and ReloadPlugin / ReloadModule classes
* PermissionModule : Implements the Permission System from [Synapse3](https://github.com/pardeike/Harmony)
* SlCommandModule : Utilises the CommandsModule to create a easy to use Command API for the SL Enviroment
