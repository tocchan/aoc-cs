using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day20 : Day
   {
      private string InputFile = "2023/inputs/20.txt";

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      
      class Pulse
      {
         public string Sender;
         public string Receiver;
         public bool IsHigh; 

         public Pulse(string from, string to, bool isHigh)
         {
            Sender = from; 
            Receiver = to; 
            IsHigh = isHigh; 
         }
      }

      class Module
      {
         public Module(string line)
         {
            (string id, string options) = line.Split(" -> ");
            
            Type = id[0];
            if (Type == 'b') {
               Id = id; 
            } else { 
               Id = id.Substring(1); 
            }

            Options = options.Split(',', StringSplitOptions.TrimEntries).ToList(); 
         }

         public char Type; 
         public string Id; 
         public List<string> Options;

         // state for different options
         public bool IsOn = false;
         public Dictionary<string, bool> ConjunctionStates = new();

         public void SendPulse(Queue<Pulse> output, bool isHigh)
         {
            foreach (string option in Options) {
               output.Enqueue(new Pulse(Id, option, isHigh)); 
            }
         }

         bool AreAllHigh()
         {
            foreach (var iter in ConjunctionStates) {
               if (!iter.Value) {
                  return false; 
               }
            }
            return true; 
         }

         public void ReceivePulse(Queue<Pulse> output, Pulse pulse)
         {
            if (Type == 'b') {
               SendPulse(output, pulse.IsHigh); 
            } else if (Type == '%') {
               if (pulse.IsHigh) {
                  return; // ignored
               } else {
                  IsOn = !IsOn; 
                  SendPulse(output, IsOn); 
               }
            } else if (Type == '&') {
               ConjunctionStates[pulse.Sender] = pulse.IsHigh; 
               bool allHigh = AreAllHigh(); 
               SendPulse(output, !allHigh); 
            }
         }

         public void Hookup(Dictionary<string, Module> lookup)
         {
            foreach (string option in Options) {
               if (lookup.ContainsKey(option)) {
                  Module m = lookup[option]; 
                  if (m.Type == '&') {
                     m.ConjunctionStates[Id] = false; 
                  }
               }
            }
         }

         public void Reset()
         {
            IsOn = false; 
            foreach (var iter in ConjunctionStates) {
               ConjunctionStates[iter.Key] = false; 
            }
         }
      }

      /*
      There are several different types of modules:

      Flip-flop modules (prefix %) are either on or off; they are initially off. 
      If a flip-flop module receives a high pulse, it is ignored and nothing happens. 
      However, if a flip-flop module receives a low pulse, it flips between on and 
      off. If it was off, it turns on and sends a high pulse. If it was on, it turns 
      off and sends a low pulse.

      Conjunction modules (prefix &) remember the type of the most recent pulse received
      from each of their connected input modules; they initially default to remembering
      a low pulse for each input. When a pulse is received, the conjunction module first
      updates its memory for that input. Then, if it remembers high pulses for all
      inputs, it sends a low pulse; otherwise, it sends a high pulse.

      There is a single broadcast module (named broadcaster). When it receives a pulse,
      it sends the same pulse to all of its destination modules.
      */

      private List<Module> Modules = new();
      private Dictionary<string, Module> Lookup = new();

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Module input = new Module(line);
            Modules.Add(input);
            Lookup.Add(input.Id, input); 
         }

         foreach (Module m in Modules) {
            m.Hookup(Lookup); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 lowSent = 0;
         Int64 highSent = 0; 

         Queue<Pulse> pulses = new(); 

         for (int i = 0; i < 1000; ++i) { 
            pulses.Enqueue(new Pulse("button", "broadcaster", false)); 
            while (pulses.Count > 0) {
               Pulse p = pulses.Dequeue();
               
               if (p.IsHigh) {
                  ++highSent; 
               } else {
                  ++lowSent; 
               }
               
               if (Lookup.ContainsKey(p.Receiver)) { 
                  Module m = Lookup[p.Receiver];
                  m.ReceivePulse(pulses, p); 
               }
            }
         }; 

         return (lowSent * highSent).ToString(); 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 ans = Solve("rx"); 

         return ans.ToString(); 
      }


      Module? GetAffectingModule(string id)
      {
         foreach (Module m in Modules) {
            if (m.Options.Contains(id)) {
               return m; 
            }
         }

         return null; 
      }

      Int64 HowLongTillOn(string module)
      {
         Queue<Pulse> pulses = new();
         Int64 buttonPresses = 0;

         foreach (Module m in Modules) {
            m.Reset();
         }

         while (true) {
            ++buttonPresses;

            pulses.Enqueue(new Pulse("button", "broadcaster", false));
            while (pulses.Count > 0) {
               Pulse p = pulses.Dequeue();
               if ((p.Sender == module) && p.IsHigh) {
                  return buttonPresses;
               }

               if (Lookup.ContainsKey(p.Receiver)) {
                  Module m = Lookup[p.Receiver];
                  m.ReceivePulse(pulses, p);
               }
            }
         }
      }

      Int64 Solve(string module)
      {
         Module m = GetAffectingModule(module)!; 
         List<Int64> values = new(); 
         foreach (var iter in m.ConjunctionStates) {
            Module feeder = Lookup[iter.Key]; 
            Int64 l = 1; 
            // if (feeder.Type == '&') {
            //   l = Solve(feeder.Id); 
            // } else {
               l = HowLongTillOn(iter.Key); 
            // }
            values.Add(l); 
         }

         return Util.LCM(values); 
      }
   }

}
