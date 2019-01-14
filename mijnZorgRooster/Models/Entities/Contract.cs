﻿using System;
using System.ComponentModel.DataAnnotations;

namespace mijnZorgRooster.Models.Entities
{
    public class Contract
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ContractID { get; set; }
        public DateTime BeginDatum { get; set; }
        public DateTime Einddatum { get; set; }
        public int ContractUren { get; set; }

        public Medewerker Medewerker { get; set; }

        // Dit is wederom iets wat berekend moet worden. Dit moet ik nog even navragen voor ik Controllers, Views en databases met connectiestrings aan ga maken.
        public int VerlofDagenPerJaar { get; set; }

        //Parttime percentage zou berekend moeten worden. Weet nog niet hoe
        public int ParttimePercentage { get; set; }

    }
}