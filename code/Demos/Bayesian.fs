﻿
//    This file is part of F-AI.
//
//    F-AI is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    F-AI is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with F-AI.  If not, see <http://www.gnu.org/licenses/>.


module Bayesian

// namespaces

open System
open FAI.Bayesian
open FAI.Loaders


let doDemoBayesian = 

    // Load traffic data set.
    let dataSetTraffic = TrafficLoader.LoadFromFile "traffic.txt" :> IObservationSet

    // Print first sample's value for 'a5'.
    let firstSample = dataSetTraffic |> Seq.head
    printfn "%f" (firstSample.TryValueForVariable "a5" |> Option.get)

    // Grab variable names.
    let variableNames = firstSample.VariableNames

    // Build a Bayesian network.
    let bn = new BayesianNetwork "Traffic"
    let prior = new DirichletDistribution (Map.ofList [ 0.,1. ; 1.,1. ; 2.,1. ; 3.,1. ])
    for variableName in variableNames do
        let dist = new DistributionSet ()
        let space = dataSetTraffic.Variables |> Map.find variableName
        let rv = new RandomVariable (variableName, space, dist)

        rv.Prior <- Some prior
        bn.AddVariable rv
    ()

    //bn.GenerateStructure Random
    bn.LearnStructure dataSetTraffic


    // Learn CPTs.
    bn.LearnDistributions dataSetTraffic

    // Test ordering.
    let ordering = bn.Variables

    // Test sampling
    let samples = { 0..20 } |> Seq.map (fun _ -> bn.Sample ()) |> Seq.toArray

    // Done.
    ()
