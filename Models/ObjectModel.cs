using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalogenPreTestAPI.Models;

public class ObjectModel
{
    public List<Double>? DivBy3 { get; set; }
    public List<Double>? DivBy5 { get; set; }
    public List<Double>? DivBy7 { get; set; }
    public List<Double>? EvenNums { get; set; }
    public List<Double>? OddNums { get; set; }
    public double Mode { get; set; }
    public double Median { get; set; }
}