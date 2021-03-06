﻿using System;

namespace RushHour.Events
{
    [Serializable]
    public class CityEventData
    {
        public bool m_eventCreated = false;
        public bool m_eventStarted = false;
        public bool m_eventEnded = false;
        public bool m_userEvent = false;
        public bool m_canBeWatchedOnTV = false;
        public ushort m_eventBuilding = 0;
        public int m_registeredCitizens = 0;
        public int m_citizensNotBothered = 0;
        public int m_totalAttemptedCitizens = 0;
        public int m_userTickets = 0;
        public float m_entryCost = 0;
        public string m_eventName = "";
        public string m_userMadeName = "";
        public string m_creationDate = "";
        public DateTime m_eventStartTime;
        public DateTime m_eventFinishTime;
        public CityEventDataIncentives[] m_incentives;
    }

    [Serializable]
    public class CityEventDataIncentives
    {
        public string name = "";
        public float returnCost = 0;
        public int itemCount = 0;
        public int boughtItems = 0;
    }
}
