public class Timestamp
{
    public int Hour;
    public int Minutes;
    public int Day;

    public Timestamp() { }

    public Timestamp(int hour, int minutes, int day)
    {
        Hour = hour;
        Minutes = minutes;
        Day = day;
    }

    public int IsPastTime(int hour, int minutes, int day)
    {
        if (Day > day) return -1;
        if (Day < day) return 1;

        if (Hour > hour) return -1;
        if (Hour < hour) return 1;

        if (Minutes > minutes) return -1;
        if (Minutes < minutes) return -1;

        return 0;
    }

    public int IsPastTimestamp(Timestamp timestamp)
    {
        return IsPastTime(timestamp.Hour, timestamp.Minutes, timestamp.Day);
    }
}
