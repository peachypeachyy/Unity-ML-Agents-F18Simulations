# Unity-ML-Agents-F18Simulations

Simulate Aircraft landings without spending any costs.

The repository features simulations of an aircraft such as an F18 on an Aircraft carrier subject to real-world environmental constraints using reinforcement learning of Proximal Policy Optimization.

Note this only contains the Assets of the Individual project and not the assets of the Entire Unity Library.


### Project Requirements :

    - Unity Editor: 2019
    - Blender: 3.5.2
    - Python: 3.7.16
    - ML-Agents: 0.16.0
    - TensorFlow: 2.11.0
    - TensorBoard: 2.11.2 
    - numpy: 1.21.6
    - keras: 2.11.0
    - cuda toolkit: 10.2.89


# Introduction and Background

Aircraft landings on Aircraft Carriers have inherently been a very expensive process. This is because the margin of error is razor thin, another factor that increases the risk is the limited space for the aircraft to take off or land on which requires precision and timing. 

Environmental factors such as the Wind Speed, Air Density, Rain and Fog add further complexities. Aircraft Carriers generally are in motion. They also have a rotational pitch and roll effect which sways the ship due to the water buoyancy making landing challenging.

Training pilots in such conditions is a difficult proposition because similar environments will not be available every time, adding to this, the costs of transporting the pilots, arranging the aircrafts, fuel and training them in such situations is very high.

In the case of an erroneous situation where the aircraft is not able to land correctly, many times they must retry the landing which adds to additional fuel costs. At times the aircraft must try a cobra landing which adds additional thrust onto the ship in order to stop the aircraft to land it in a desired spot.

Simulations can help in such situations where physical demonstrations may prove expensive. They help providing a real-world or near real-world experience to trainees helping them get up to speed without incurring a huge cost.

# Methodology

Domain Driven design is used so that each object has it's own properties and it's own methods.

The Entire code has been written in C# which then indexes the ML-Agents implementation written in python.
The project uses Tensorflow to perform reinforcement learning using Proximal Policy Optimization.

The following is the rough code structure :

![My Image](https://github.com/peachypeachyy/portfolio-contents/blob/main/rl-aircraft_ai/supporting_assets/code_structure.png)


The main goal of the project is maximize the cumulative reward structure.
The code for implementation is in the Assets folder.

# Results

The Following shows the successful landing of the aircraft on the aircraft carrier

![My Image](https://github.com/peachypeachyy/portfolio-contents/blob/main/rl-aircraft_ai/supporting_assets/aircraft_2nd_land.png)

The front view camera of the aircraft on landing is as follows :

![My Image](https://github.com/peachypeachyy/portfolio-contents/blob/main/rl-aircraft_ai/supporting_assets/Aircraft_successfull_landing.png)


The metrics and evaluation show a steady rise in the cumulative reward over time indicating successful reinforcement learning

![My Image](https://github.com/peachypeachyy/portfolio-contents/blob/main/rl-aircraft_ai/supporting_assets/Cumulative_Reward_Aircraft_Landing-1.png)


The learning rate, gradient descent and the number of hidden neural networks were tweaked to provide superior performance over previous runs.

![My Image](https://github.com/peachypeachyy/portfolio-contents/blob/main/rl-aircraft_ai/supporting_assets/Cumulative_Reward_al_20_x_al_18.png)

It is visible that the Green line is rising steadily and is much more fine-tuned compared to the yellow line which is very choppy.

# Conclusion

Therefore using reinforcement learning with Tensorflow interfacing with Unity as a simulation engine, we were able to train the agent with real-life circumstances to provide a near real-time simulation.

