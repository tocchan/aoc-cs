using AoC; 

Day? day = Day.Create( "2024", args.FirstOrDefault(), typeof(AoC2024.Day24) );


Util.WriteLine(""); 
Util.WriteLine("[blue]_._     _,-'\"\"`-._"); 
Util.WriteLine("[blue],-.`._,'(       |\\`-/|"); 
Util.WriteLine("[blue]   `-.-' \\ )-`( , [yellow]o o[-])"); 
Util.WriteLine("[white]---------[blue]`-    \\`_`\"'- [+white]AoC.2024." + day.GetType().Name + " [white]---------"); 
Util.WriteLine(""); 


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


