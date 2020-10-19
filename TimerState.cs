using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace TimeTable
{
    public enum State
    {
        Pause,
        Break,
        Work
    }
    
    [Serializable]
    public struct Shift
    {
        [JsonInclude] public DateTime Start;
        [JsonInclude] public DateTime End;

        [JsonIgnore] public TimeSpan Duration => (End != default ? End : DateTime.Now) - Start;
    }
    
    [Serializable]
    public struct TimerState
    {
        private static readonly TimeSpan _workDay = TimeSpan.FromHours(8);

        [JsonInclude] public List<Shift>? Shifts;

        [JsonIgnore] public bool IsWorking => Shifts?.Count > 0 && Shifts[^1].End == default;

        [JsonIgnore] public TimeSpan TotalWorkDuration 
            => Shifts?.Aggregate(TimeSpan.Zero, (span, shift) => span + shift.Duration) ?? default;

        [JsonIgnore] public TimeSpan WorkDuration => IsWorking 
            ? DateTime.Now - Shifts![^1].Start : default;

        [JsonIgnore] public TimeSpan TotalBreakDuration 
            => LastEntry - FirstEntry - TotalWorkDuration;

        [JsonIgnore] public DateTime FirstEntry => Shifts?[0].Start ?? default;

        [JsonIgnore] public DateTime LastEntry
        {
            get
            {
                if (Shifts == null || Shifts.Count == 0) return default;
                var shiftEnd = Shifts[^1].End;
                return shiftEnd != default ? shiftEnd : DateTime.Now;
            }
        }

        [JsonIgnore] public TimeSpan Flex => TotalWorkDuration - _workDay;
        [JsonIgnore] public TimeSpan BreakDuration => !IsWorking ? DateTime.Now - LastEntry : default;
        [JsonIgnore] public TimeSpan CurrentSession => IsWorking ? WorkDuration : BreakDuration;

        public void StartWorking()
        {
            Shifts ??= new List<Shift>();
            if (!IsWorking)
            {
                Shifts.Add(new Shift {Start = DateTime.Now});
            }
        }

        public void StopWorking()
        {
            if (IsWorking)
            {
                var currentShift = Shifts![^1];
                currentShift.End = DateTime.Now;
                Shifts[^1] = currentShift;
            }
        }
        
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static TimerState FromJson(string? json)
        {
            return (json != null 
                ? JsonSerializer.Deserialize<TimerState>(json) 
                : default)!;
        }
    }
}