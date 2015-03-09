﻿using Codebreak.Service.World.Database.Structure;
using Codebreak.Service.World.Game.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Job
{
    /// <summary>
    /// 
    /// </summary>
    public enum JobIdEnum
    {
        JOB_NONE = 0,
        JOB_BASE = 1,
        JOB_BUCHERON = 2,
        JOB_FORGEUR_EPEES = 11,
        JOB_SCULPTEUR_ARCS = 13,
        JOB_FORGEUR_DE_MARTEAUX = 14,
        JOB_CORDONNIER = 15,
        JOB_BIJOUTIER = 16,
        JOB_FORGEUR_DE_DAGUES = 17,
        JOB_SCULPTEUR_DE_BATONS = 18,
        JOB_SCULPTEUR_DE_BAGUETTES = 19,
        JOB_FORGEUR_DE_PELLES = 20,
        JOB_MINEUR = 24,
        JOB_BOULANGER = 25,
        JOB_ALCHIMISTE = 26,
        JOB_TAILLEUR = 27,
        JOB_PAYSAN = 28,
        JOB_FORGEUR_DE_HACHES = 31,
        JOB_PECHEUR = 36,
        JOB_CHASSEUR = 41,
        JOB_FORGEMAGE_DE_DAGUES = 43,
        JOB_FORGEMAGE_EPEES = 44,
        JOB_FORGEMAGE_DE_MARTEAUX = 45,
        JOB_FORGEMAGE_DE_PELLES = 46,
        JOB_FORGEMAGE_DE_HACHES = 47,
        JOB_SCULPTEMAGE_ARCS = 48,
        JOB_SCULPTEMAGE_DE_BAGUETTES = 49,
        JOB_SCULPTEMAGE_DE_BATONS = 50,
        JOB_BOUCHER = 56,
        JOB_POISSONNIER = 58,
        JOB_FORGEUR_DE_BOUCLIERS = 60,
        JOB_CORDOMAGE = 62,
        JOB_JOAILLOMAGE = 63,
        JOB_COSTUMAGE = 64,
        JOB_BRICOLEUR = 65,
        JOB_JOAILLIER = 66,
        JOB_BIJOUTIER_1 = 67,
        JOB_PAYSAN_1 = 70,
        JOB_PAYSAN_2 = 71,
        JOB_COUPE = 72,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SkillIdEnum
    {
        SKILL_ACCEDER = 175,
        SKILL_ACHETER = 97,
        SKILL_ACHETER_1 = 176,
        SKILL_ACTIONNER = 179,
        SKILL_AMELIORER_DES_BOTTES = 163,
        SKILL_AMELIORER_UN_ANNEAU = 168,
        SKILL_AMELIORER_UN_CHAPEAU = 166,
        SKILL_AMELIORER_UN_SAC = 167,
        SKILL_AMELIORER_UNE_AMULETTE = 169,
        SKILL_AMELIORER_UNE_CAPE = 165,
        SKILL_AMELIORER_UNE_CEINTURE = 164,
        SKILL_BRICOLER = 171,
        SKILL_BRISER_UN_OBJET = 181,
        SKILL_COLLECTER_FER = 24,
        SKILL_COLLECTER_CUIVRE = 25,
        SKILL_COLLECTER_BRONZE = 26,
        SKILL_COLLECTER_KOBALTE = 28,
        SKILL_COLLECTER_ARGENT = 29,
        SKILL_COLLECTER_OR = 30,
        SKILL_COLLECTER_BAUXITE = 31,
        SKILL_COLLECTER_ETAIN = 55,
        SKILL_COLLECTER_MANGANESE = 56,
        SKILL_COLLECTER_DOLOMITE = 161,
        SKILL_COLLECTER_SILICATE = 162,
        SKILL_COLLECTER_11 = 192,
        SKILL_CONCASSER_DES_RESSOURCES = 121,
        SKILL_CONFECTIONNER_DES_BOTTES = 13,
        SKILL_CONFECTIONNER_UNE_CEINTURE = 14,
        SKILL_CONFECTIONNER_UNE_CLEF = 182,
        SKILL_CONSULTER = 170,
        SKILL_COUDRE_UN_CHAPEAU = 63,
        SKILL_COUDRE_UN_SAC = 123,
        SKILL_COUDRE_UNE_CAPE = 64,
        SKILL_COUPER_FRENE = 6,
        SKILL_COUPER_CHENE = 10,
        SKILL_COUPER_IF = 33,
        SKILL_COUPER_EBENE = 34,
        SKILL_COUPER_ORME = 35,
        SKILL_COUPER_ERABLE = 37,
        SKILL_COUPER_CHARME = 38,
        SKILL_COUPER_CHATAIGNER = 39,
        SKILL_COUPER_NOYER = 40,
        SKILL_COUPER_MERISIER = 41,
        SKILL_COUPER_BOMBU = 139,
        SKILL_COUPER_OLIVIOLET = 141,
        SKILL_COUPER_BAMBOU = 154,
        SKILL_COUPER_BAMBOUSACRE = 155,
        SKILL_COUPER_BAMBOUSOMBRE = 158,
        SKILL_COUPER_KALIPTUS = 174,
        SKILL_COUPER_16 = 190,
        SKILL_CREER_UN_ANNEAU = 11,
        SKILL_CREER_UNE_AMULETTE = 12,
        SKILL_CUEILLIR_LIN = 68,
        SKILL_CUEILLIR_CHANVRE = 69,
        SKILL_CUEILLIR_TREFLE = 71,
        SKILL_CUEILLIR_MENTHE = 72,
        SKILL_CUEILLIR_ORCHIDEE = 73,
        SKILL_CUEILLIR_EDELWEISS = 74,
        SKILL_CUEILLIR_PANDOUILLE = 160,
        SKILL_CUEILLIR_7 = 188,
        SKILL_CUIRE_DU_PAIN = 27,
        SKILL_DEVERROUILLER = 100,
        SKILL_DEVERROUILLER_1 = 106,
        SKILL_EGRENER = 122,
        SKILL_ENTRER = 84,
        SKILL_EPLUCHER = 22,
        SKILL_FAIRE_DE_LA_BIERE = 96,
        SKILL_FAIRE_DES_BONBONS = 109,
        SKILL_FAUCHER_BLE = 45,
        SKILL_FAUCHER_HOUBLON = 46,
        SKILL_FAUCHER_LIN = 50,
        SKILL_FAUCHER_SEIGLE = 52,
        SKILL_FAUCHER_ORGE = 53,
        SKILL_FAUCHER_CHANVRE = 54,
        SKILL_FAUCHER_AVOINE = 57,
        SKILL_FAUCHER_MALT = 58,
        SKILL_FAUCHER_RIZ = 159,
        SKILL_FAUCHER_9 = 191,
        SKILL_FILER = 95,
        SKILL_FONDRE = 32,
        SKILL_FORGER_UN_BOUCLIER = 156,
        SKILL_FORGER_UN_MARTEAU = 19,
        SKILL_FORGER_UNE_DAGUE = 18,
        SKILL_FORGER_UNE_EPEE = 20,
        SKILL_FORGER_UNE_FAUX = 66,
        SKILL_FORGER_UNE_HACHE = 65,
        SKILL_FORGER_UNE_PELLE = 21,
        SKILL_FORGER_UNE_PIOCHE = 67,
        SKILL_FOUILLER = 153,
        SKILL_INVOQUER_UNE_FEE = 151,
        SKILL_JOUER = 150,
        SKILL_MODIFIER_LE_PRIX_DE_VENTE = 108,
        SKILL_MODIFIER_LE_PRIX_DE_VENTE_1 = 178,
        SKILL_MOUDRE = 47,
        SKILL_OUVRIR = 104,
        SKILL_POLIR_UNE_PIERRE = 48,
        SKILL_PREPARER = 132,
        SKILL_PREPARER_UN_POISSON = 135,
        SKILL_PREPARER_UNE_VIANDE = 134,
        SKILL_PREPARER_UNE_POTION = 23,
        SKILL_PUISER = 102,
        SKILL_PECHER_GOUJON = 124,
        SKILL_PECHER_TRUITE = 125,
        SKILL_PECHER_POISSONCHATON = 126,
        SKILL_PECHER_BROCHET = 127,
        SKILL_PECHER_GREUVETTE = 128,
        SKILL_PECHER_CRABESOURIMI = 129,
        SKILL_PECHER_POISSONPANE = 130,
        SKILL_PECHER_SARDINEBRILLANTE = 131,
        SKILL_PECHER_PICHONEUDCOMPET = 136,
        SKILL_PECHER_KRALAMOURE = 140,
        SKILL_PECHER_SARDINEBRILLANTE_1 = 189,
        SKILL_PECHER_KOINKOIN = 152,
        SKILL_RAMASSER = 42,
        SKILL_REFORGER_UN_MARTEAU = 116,
        SKILL_REFORGER_UNE_DAGUE = 1,
        SKILL_REFORGER_UNE_EPEE = 113,
        SKILL_REFORGER_UNE_HACHE = 115,
        SKILL_REFORGER_UNE_PELLE = 117,
        SKILL_RESCULPTER_UN_ARC = 118,
        SKILL_RESCULPTER_UN_BATON = 120,
        SKILL_RESCULPTER_UNE_BAGUETTE = 119,
        SKILL_REPARER_UN_ARC = 149,
        SKILL_REPARER_UN_BATON = 147,
        SKILL_REPARER_UN_MARTEAU = 144,
        SKILL_REPARER_UNE_BAGUETTE = 148,
        SKILL_REPARER_UNE_DAGUE = 142,
        SKILL_REPARER_UNE_EPEE = 145,
        SKILL_REPARER_UNE_HACHE = 143,
        SKILL_REPARER_UNE_PELLE = 146,
        SKILL_SAUVEGARDER = 44,
        SKILL_SCIER = 101,
        SKILL_SCULPTER_UN_ARC = 15,
        SKILL_SCULPTER_UN_BATON = 17,
        SKILL_SCULPTER_UNE_BAGUETTE = 16,
        SKILL_SE_FAIRE_TRANSPORTER = 157,
        SKILL_SE_RENDRE_A_INCARNAM = 183,
        SKILL_SE_REGENERER = 62,
        SKILL_SE_REGENERER_1 = 111,
        SKILL_SORTIR = 187,
        SKILL_UTILISER_ZAAP = 114,
        SKILL_UTILISER = 184,
        SKILL_UTILISER_ETABLI = 110,
        SKILL_VENDRE = 98,
        SKILL_VENDRE_1 = 177,
        SKILL_VERROUILLER = 81,
        SKILL_VERROUILLER_1 = 105,
        SKILL_VIDER_POISSON = 133,
        SKILL_REF_COFFREMAISON = 185,
        SKILL_REF_MAISON = 186,

        SKILL_USE_PHOENIX = 300,
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class JobTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public JobIdEnum Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public JobIdEnum ParentJobId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<JobSkill> m_skills;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        public JobTemplate(JobIdEnum id, JobIdEnum parentId = JobIdEnum.JOB_NONE, params JobSkill[] skills)
        {
            Id = id;
            ParentJobId = parentId;
            m_skills = new List<JobSkill>(skills);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasSkill(CharacterEntity character, int id, int level)
        {
            return m_skills.Any(skill => (int)skill.SkillId == id && skill.Usable(character, level));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public JobSkill GetSkill(CharacterEntity character, int id, int level)
        {
            return m_skills.Find(skill => (int)skill.SkillId == id && skill.Usable(character, level));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasSkill(CharacterEntity character, SkillIdEnum id, int level)
        {
            return HasSkill(character, (int)id, level);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public bool HasTool(int templateId)
        {
            return m_skills.Any(skill => skill.Tools.Contains(templateId));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SerializeAs_SkillListMessage(CharacterJobDAO job, StringBuilder message)
        {
            message.Append((int)Id).Append(';');
            foreach(var skill in m_skills)
            {
                if(skill.ObtainLevel <= job.Level)
                {
                    skill.SerializeAs_SkillListMessage(job, message);
                    message.Append(',');
                }
            }
            if(m_skills.Count > 0)
                message.Remove(message.Length - 1, 1);
            message.Append('|');
        }
    }
}
