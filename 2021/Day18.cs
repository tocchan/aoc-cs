using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    public class SnailFishNumber
    {
        //----------------------------------------------------------------------------------------------
        public SnailFishNumber()
        {

        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber( int val )
        {
            Parent = null; 
            Left = null; 
            Right = null; 
            Value = val; 
        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber( SnailFishNumber parent, int val )
            : this(val)
        {
            Left = null; 
            Right = null; 
            Value = val; 
            Parent = parent; 
        }

        //----------------------------------------------------------------------------------------------
        private static int FindSeperatorIndex( string line )
        {
            int bracketCount = 0; 
            for (int i = 0; i < line.Length; ++i)
            {
                if (line[i] == '[')
                {
                    ++bracketCount; 
                }
                else if (line[i] == ']')
                {
                    --bracketCount; 
                }
                else if ((line[i] == ',') && (bracketCount == 0))
                {
                    return i; 
                }
            }

            return -1; // please don't get here
        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber( string input )
        {
            if (input[0] == '[')
            {
                input = input.Substring( 1, input.Length - 2 ); // remove brackets
                int sepIdx = FindSeperatorIndex( input ); 

                string partA = input.Substring( 0, sepIdx ); 
                string partB = input.Substring( sepIdx + 1 ); 

                Left = new SnailFishNumber( this, partA ); 
                Right = new SnailFishNumber( this, partB ); 
            }
            else
            {
                Value = int.Parse( input ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber( SnailFishNumber parent, string input )
            : this(input)
        {
            Parent = parent; 
        }

        //----------------------------------------------------------------------------------------------
        public static SnailFishNumber operator +( SnailFishNumber l, SnailFishNumber r )
        {
            string newStr = $"[{l},{r}]"; 
            SnailFishNumber result = new SnailFishNumber(newStr); 
            result.Reduce(); 

            return result; 
        }

        //----------------------------------------------------------------------------------------------
        public bool IsLeafNode()
        {
            return (Left == null) || (Right == null); 

        }

        //----------------------------------------------------------------------------------------------
        private SnailFishNumber? GetLeftSibling( SnailFishNumber child )
        {
            // terrible name, basically just get right most leaf 
            if (Left == child)
            {
                if (Parent == null)
                {
                    return null; 
                }
                else
                {
                    return Parent.GetLeftSibling( this ); 
                }
            }
            else
            {
                SnailFishNumber node = Left!; 
                while (node.Right != null)
                {
                    node = node.Right; 
                }

                return node; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private SnailFishNumber? GetRightSibling( SnailFishNumber child )
        {
            // terrible name, basically just get right most leaf 
            if (Right == child)
            {
                if (Parent == null)
                {
                    return null; 
                }
                else
                {
                    return Parent.GetRightSibling( this ); 
                }
            }
            else
            {
                SnailFishNumber node = Right!; 
                while (node.Left != null)
                {
                    node = node.Left; 
                }

                return node; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private void Explode()
        {
            int leftValue = Left!.Value; 
            int rightValue = Right!.Value;

            SnailFishNumber? leftSibling = Parent!.GetLeftSibling( this ); 
            SnailFishNumber? rightSibling = Parent.GetRightSibling( this ); 
            if (leftSibling != null)
            {
                leftSibling.Value += leftValue; 
            }

            if (rightSibling != null)
            {
                rightSibling.Value += rightValue; 
            }

            Value = 0; 
            Left = null; 
            Right = null; 
        }

        //----------------------------------------------------------------------------------------------
        private void Split()
        {
            int leftValue = Value / 2; 
            int rightValue = Value - leftValue; 
            Left = new SnailFishNumber( this, leftValue ); 
            Right = new SnailFishNumber( this, rightValue ); 
        }

        //----------------------------------------------------------------------------------------------
        private bool CheckForExplodes( int depth )
        {
            if (IsLeafNode())
            {
                return false; 
            } 
            else 
            {
                if (depth == 4)
                {
                    Explode(); 
                    // Util.WriteLine( $"Exp: [cyan]{GetRoot()}" ); 
                    return true; 
                }
                else
                {
                    return Left!.CheckForExplodes( depth + 1 ) || Right!.CheckForExplodes( depth + 1 ); 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        private bool CheckForSplits()
        {
            if (IsLeafNode())
            {
                if (Value >= 10)
                {
                    Split(); 
                    return true; 
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return Left!.CheckForSplits() || Right!.CheckForSplits(); 
            }
        }



        //----------------------------------------------------------------------------------------------
        public void Reduce()
        {
            // keep trying until a rule doesn't apply
            while (CheckForExplodes( 0 ) || CheckForSplits()) 
            {
               // Util.WriteLine( $"[cyan]{this}" ); // reduce step
            }
        }

        //----------------------------------------------------------------------------------------------
        public int GetMagnitude()
        {
            if (IsLeafNode())
            {
                return Value; 
            }
            else
            {
                return 3 * Left!.GetMagnitude() + 2 * Right!.GetMagnitude(); 
            }

        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber GetRoot()
        {
            SnailFishNumber root = this; 
            while (root.Parent != null)
            {
                root = root.Parent; 
            }

            return root; 
        }
        //----------------------------------------------------------------------------------------------
        public override string ToString()
        {
            if (IsLeafNode())
            {
                return Value.ToString(); 
            }
            else
            {
                return $"[{Left},{Right}]"; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public SnailFishNumber? Parent = null; 
        public SnailFishNumber? Left = null; 
        public SnailFishNumber? Right = null; 
        public int Value = 0; // leaf node
    }

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day18 : Day
    {
        private string InputFile = "2021/inputs/18.txt"; 
        private List<SnailFishNumber> InputNumbers = new List<SnailFishNumber>(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            foreach (string line in lines)
            {
                InputNumbers.Add( new SnailFishNumber(line) ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            SnailFishNumber answer = InputNumbers[0];
            for (int i = 1; i < InputNumbers.Count; ++i)
            {
                answer += InputNumbers[i]; 
                // Util.WriteLine( answer.ToString() ); 
            }
            
            // Util.WriteLine("------"); 
            // Util.WriteLine( answer.ToString() ); 
            return answer.GetMagnitude().ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int bestMag = 0; 
            for (int i = 0; i < InputNumbers.Count; ++i)
            {
                for (int j = 0; j < InputNumbers.Count; ++j)
                {
                    if (i != j)
                    {
                        bestMag = Math.Max( bestMag, (InputNumbers[i] + InputNumbers[j]).GetMagnitude() ); 
                        bestMag = Math.Max( bestMag, (InputNumbers[j] + InputNumbers[i]).GetMagnitude() ); 
                    }
                }
            }
            
            return bestMag.ToString();
        }
    }
}
