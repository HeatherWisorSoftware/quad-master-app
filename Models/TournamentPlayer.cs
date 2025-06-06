﻿namespace QuadMasterApp.Models
{
    public class TournamentPlayer
    {
        //primary key
        public int Id { get; set; }

        //join entitiy Player (foreign key)
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        //join entity Tournament (foreign key)
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }


        //Quad Assignment
        public int? QuadId { get; set; }
        public Quad Quad { get; set; }

        //Helper property to check if player is assigned to a quad
        public bool IsAssigned => QuadId.HasValue;
    }
}
