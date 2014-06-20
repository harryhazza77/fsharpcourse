// --------------------------------------------------------
// This F# dojo is directly inspired by the Digit Recognizer
// competition from Kaggle.com: http://www.kaggle.com/c/digit-recognizer
//
// Based on F# coding DoJo by Mathias Brandewinder. See:
// * http://www.slideshare.net/mathias-brandewinder/fsharp-and-machine-learning-dojo
// * https://gist.github.com/mathias-brandewinder/5558573
// --------------------------------------------------------

#r "lib/FSharp.Data.dll"
open FSharp.Data.Csv
open System.Drawing
open System.Windows.Forms

let data = CsvFile.Load(__SOURCE_DIRECTORY__ + "\\data\\digitssample.csv").Cache()

/// Get the label (actual digit) for a given row
let getLabel (row:CsvRow) = 
  row.Columns.[0] |> int

/// Get a float array with all the pixels for the row
let getDataArray (row:CsvRow) = 
  row.Columns.[1 .. 784] |> Array.map float

/// Given a float array, display it in a form
let showDigit (data:float[]) = 
  let bmp = new Bitmap(28, 28)
  for i in 0 .. 783 do
    let value = int data.[i]
    let color = Color.FromArgb(value, value, value)
    bmp.SetPixel(i % 28, i / 28, color)
  let frm = new Form(Visible = true, ClientSize = Size(280, 280))
  let img = new PictureBox(Image = bmp, Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage)
  frm.Controls.Add(img)

// Examples
data.Data |> Seq.length
data.Data |> Seq.head |> getDataArray
data.Data |> Seq.head |> getDataArray |> showDigit
data.Data |> Seq.head |> getLabel

// --------------------------------------------------------
// TODO: Calculate distance between two arrays
// --------------------------------------------------------

let aggreageDistance (sample1:float[]) (sample2:float[]) =
  
  // TODO: Compute the difference between two images. Given:
  //
  //   [a1; a2; a3] and [b1; b2; b3], we can calculate:
  //   sqrt ( (a1-b1)^2 + (a2-b2)^2 + (a3-b3)^2 )
  //
  // Or try to find another, better definition of distance!
  //
  0.0

let zero1 = data.Data |> Seq.nth 1 |> getDataArray
let zero2 = data.Data |> Seq.nth 5 |> getDataArray
let one1 = data.Data |> Seq.nth 0 |> getDataArray
let one2 = data.Data |> Seq.nth 2 |> getDataArray

// The first two should be smaller
aggreageDistance zero1 zero2
aggreageDistance one1 one2

// But this one should be larger
aggreageDistance one1 zero1

// --------------------------------------------------------
// TODO: Pick the closest average sample
// --------------------------------------------------------

let classify row =
  let data = getDataArray row
  // TODO: Iterate over the inputs in 'data.Data'
  // and find the one that is closest to the input 'data'
  // (and return the key - label - of the sample)
  0

// --------------------------------------------------------
// Test the classifier using the samples
// --------------------------------------------------------

let test = CsvFile.Load(__SOURCE_DIRECTORY__ + "\\data\\digitscheck.csv").Cache()

for input in test.Data |> Seq.take 10 do
  let got = classify input
  let actual = getLabel input
  printfn "Actual %d, got %A" actual got

// Calculate how well our digit recognizer works!
test.Data |> Seq.countBy (fun input ->
  classify input = getLabel input)

// --------------------------------------------------------
// BONUS: Calculate "average" samples
// --------------------------------------------------------

let samples = 
  // TODO: Take all the training samples (or perhaps just
  // the representative ones) and create an "average" sample
  // by blending them all together. (You might need 
  // Seq.groupBy to create groups with the same label and
  // then Array.init & Array.get to create a new image that
  // is the average of all the images. Or you can first write
  // a function that averages two arrays.)
  []

// Show the average samples
// (WARNING: Opens lots of new windows!)
for i in 0 .. 9 do
  samples.[i] |> snd |> showDigit

// TODO: Once you have "averaged" samples, change your classify
// function so that it finds the nearest averaged sample, rather
// than finding the nearest digit in the actual data set