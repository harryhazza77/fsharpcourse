open System.Drawing
open System.Windows.Forms

// Create basic user interface
let form = new Form(Visible=true)
let red = new Panel(Width=50, Height=50, Top=10, Left=10)
let orange = new Panel(Width=50, Height=50, Top=70, Left=10)
let green = new Panel(Width=50, Height=50, Top=130, Left=10)
form.Controls.AddRange([|red; orange; green|])

// Representing traffic light colors
type TrafficLight = Red | Orange | Green

// Display the specified traffic light
let displayTraffic light = 
  red.BackColor <- Color.Gray
  orange.BackColor <- Color.Gray
  green.BackColor <- Color.Gray
  match light with
  | Red -> red.BackColor <- Color.Red
  | Orange -> orange.BackColor <- Color.Orange
  | Green -> green.BackColor <- Color.Green

// Display red traffic light
displayTraffic Red

do Application.Run(form)