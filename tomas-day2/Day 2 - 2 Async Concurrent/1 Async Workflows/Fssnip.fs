// ------------------------------------------------------------------
// Utilities - generating F# Snippets ID from snippet numbers

module FsSnip
open System

let createId i =
  let alphabet = 
    [ '0' .. '9' ] @ [ 'a' .. 'z' ] @ [ 'A' .. 'Z' ] 
    |> Array.ofList
  let rec mangle acc = function
    | 0 -> new String(acc |> Array.ofList)
    | n -> let d, r = Math.DivRem(n, alphabet.Length)
           mangle (alphabet.[r]::acc) d
  mangle [] i

