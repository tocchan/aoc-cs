using AoC; 

Day? day = Day.Create( "2023", args.FirstOrDefault(), typeof(AoC2023.Day07) );


Util.WriteLine( @"[+white]                       /)"); 
Util.WriteLine( @"[+white]              /\___/\ (("); 
Util.WriteLine( @"[+white]              \`[yellow]@[-]_[yellow]@[+white]'/  ))"); 
Util.WriteLine( @"[+white]              {_:Y:.}_//" ); 
Util.WriteLine( @"[+white]    ----------{_}^-'{_}----------"); 
Util.WriteLine( @"[  cyan]           AoC.2023." + day.GetType().Name    ); 
Util.WriteLine(  "[+white]    -----------------------------\n" ); 

string answerA; 
string answerB; 

// Parse Input round
{
    using var t = new ScopeTimer(" Parsing Input"); 
    day.ParseInput(); 
}
Console.WriteLine(); 

// Run Part A
{
    using var t = new ScopeTimer(" Part A"); 
    answerA = day.RunA(); 
}
Util.WriteLine($" > Answer A: [+white]{answerA}\n");

// Run Part B
{
    using var t = new ScopeTimer(" Part B"); 
    answerB = day.RunB(); 
}
Util.WriteLine($" > Answer B: [+white]{answerB}\n"); 

// Copy answer to clipboard out of laziness;
if (!string.IsNullOrEmpty(answerB))
{
    Clipboard.SetText(answerB); 
}
else if (!string.IsNullOrEmpty(answerA))
{
    Clipboard.SetText(answerA); 
}


