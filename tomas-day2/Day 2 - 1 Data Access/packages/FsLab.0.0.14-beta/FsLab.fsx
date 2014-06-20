#nowarn "211"
#I "packages/Deedle.1.0.0/lib/net40"
#I "packages/Deedle.RPlugin.1.0.0/lib/net40"
#I "packages/FSharp.Charting.0.90.6/lib/net40"
#I "packages/FSharp.Data.2.0.8/lib/net40"
#I "packages/MathNet.Numerics.3.0.0-beta01/lib/net40"
#I "packages/MathNet.Numerics.FSharp.3.0.0-beta01/lib/net40"
#I "packages/RProvider.1.0.9/lib"
#I "packages/R.NET.1.5.5/lib/net40"
#I "packages/RDotNet.FSharp.0.1.2.1/lib/net40"
#I "../packages/Deedle.1.0.0/lib/net40"
#I "../packages/Deedle.RPlugin.1.0.0/lib/net40"
#I "../packages/FSharp.Charting.0.90.6/lib/net40"
#I "../packages/FSharp.Data.2.0.8/lib/net40"
#I "../packages/MathNet.Numerics.3.0.0-beta01/lib/net40"
#I "../packages/MathNet.Numerics.FSharp.3.0.0-beta01/lib/net40"
#I "../packages/RProvider.1.0.9/lib"
#I "../packages/R.NET.1.5.5/lib/net40"
#I "../packages/RDotNet.FSharp.0.1.2.1/lib/net40"
#I "../../packages/Deedle.1.0.0/lib/net40"
#I "../../packages/Deedle.RPlugin.1.0.0/lib/net40"
#I "../../packages/FSharp.Charting.0.90.6/lib/net40"
#I "../../packages/FSharp.Data.2.0.8/lib/net40"
#I "../../packages/MathNet.Numerics.3.0.0-beta01/lib/net40"
#I "../../packages/MathNet.Numerics.FSharp.3.0.0-beta01/lib/net40"
#I "../../packages/RProvider.1.0.9/lib"
#I "../../packages/R.NET.1.5.5/lib/net40"
#I "../../packages/RDotNet.FSharp.0.1.2.1/lib/net40"
#I "../../../packages/Deedle.1.0.0/lib/net40"
#I "../../../packages/Deedle.RPlugin.1.0.0/lib/net40"
#I "../../../packages/FSharp.Charting.0.90.6/lib/net40"
#I "../../../packages/FSharp.Data.2.0.8/lib/net40"
#I "../../../packages/MathNet.Numerics.3.0.0-beta01/lib/net40"
#I "../../../packages/MathNet.Numerics.FSharp.3.0.0-beta01/lib/net40"
#I "../../../packages/RProvider.1.0.9/lib"
#I "../../../packages/R.NET.1.5.5/lib/net40"
#I "../../../packages/RDotNet.FSharp.0.1.2.1/lib/net40"
#r "Deedle.dll"
#r "System.Windows.Forms.DataVisualization.dll"
#r "FSharp.Charting.dll"
#r "FSharp.Data.dll"
namespace FsLab

module FsiAutoShow = 
  open FSharp.Charting

  fsi.AddPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> 
    "\n" + (printer.Format()))
  fsi.AddPrinter(fun (ch:FSharp.Charting.ChartTypes.GenericChart) -> 
    ch.ShowChart(); "(Chart)")

namespace FSharp.Charting
open FSharp.Charting
open Deedle

[<AutoOpen>]
module FsLabExtensions =
  type FSharp.Charting.Chart with
    static member Line(data:Series<'K, 'V>, ?Name, ?Title, ?Labels, ?Color, ?XTitle, ?YTitle) =
      Chart.Line(Series.observations data, ?Name=Name, ?Title=Title, ?Labels=Labels, ?Color=Color, ?XTitle=XTitle, ?YTitle=YTitle)
    static member Column(data:Series<'K, 'V>, ?Name, ?Title, ?Labels, ?Color, ?XTitle, ?YTitle) =
      Chart.Column(Series.observations data, ?Name=Name, ?Title=Title, ?Labels=Labels, ?Color=Color, ?XTitle=XTitle, ?YTitle=YTitle)
    static member Pie(data:Series<'K, 'V>, ?Name, ?Title, ?Labels, ?Color, ?XTitle, ?YTitle) =
      Chart.Pie(Series.observations data, ?Name=Name, ?Title=Title, ?Labels=Labels, ?Color=Color, ?XTitle=XTitle, ?YTitle=YTitle)
    static member Area(data:Series<'K, 'V>, ?Name, ?Title, ?Labels, ?Color, ?XTitle, ?YTitle) =
      Chart.Area(Series.observations data, ?Name=Name, ?Title=Title, ?Labels=Labels, ?Color=Color, ?XTitle=XTitle, ?YTitle=YTitle)
    static member Bar(data:Series<'K, 'V>, ?Name, ?Title, ?Labels, ?Color, ?XTitle, ?YTitle) =
      Chart.Bar(Series.observations data, ?Name=Name, ?Title=Title, ?Labels=Labels, ?Color=Color, ?XTitle=XTitle, ?YTitle=YTitle)
