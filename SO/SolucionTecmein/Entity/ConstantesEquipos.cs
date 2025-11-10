namespace Entity
{
    public static class ConstantesEquipos
    {
        public static class TipoEquipo
        {
            public const string COMERCIAL_CODIGO = "COME";
            public const string COMERCIAL_DESCRIPCION = "A. Comercial";
            public const string RESIDENCIAL_CODIGO = "RESI";
            public const string RESIDENCIAL_DESCRIPCION = "A. Residencial";
            public const string MONTAPLATOS_CODIGO = "MPLA";
            public const string MONTAPLATOS_DESCRIPCION = "A. Montaplatos";
            public const string HOSPITAL_CODIGO = "HOSP";
            public const string HOSPITAL_DESCRIPCION = "A. Para Hospital";
            public const string MONTACARGAS_CODIGO = "MCAR";
            public const string MONTACARGAS_DESCRIPCION = "A. Montacargas";
            public const string MONTACOCHES_CODIGO = "MCOC";
            public const string MONTACOCHES_DESCRIPCION = "A. Montacoches";
            public const string ELECTRICAS_CODIGO = "ESEL";
            public const string ELECTRICAS_DESCRIPCION = "E. Eléctricas";
            public const string PASARELAS_RODANTES_CODIGO = "PARO";
            public const string PASARELAS_RODANTES_DESCRIPCION = "Pas. Rodantes";
            public const string PANORAMICO_360_CODIGO = "P360";
            public const string PANORAMICO_360_DESCRIPCION = "Panoramico 360";
            public const string PANORAMICO_270_CODIGO = "P270";
            public const string PANORAMICO_270_DESCRIPCION = "Panoramico 270";
            public const string PANORAMICO_180_CODIGO = "P180";
            public const string PANORAMICO_180_DESCRIPCION = "Panoramico 180";
            public const string PANORAMICO_90_CODIGO = "PA90";
            public const string PANORAMICO_90_DESCRIPCION = "Panoramico 90";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { COMERCIAL_CODIGO, COMERCIAL_DESCRIPCION },
                { RESIDENCIAL_CODIGO, RESIDENCIAL_DESCRIPCION },
                { MONTAPLATOS_CODIGO, MONTAPLATOS_DESCRIPCION },
                { HOSPITAL_CODIGO, HOSPITAL_DESCRIPCION },
                { MONTACARGAS_CODIGO, MONTACARGAS_DESCRIPCION },
                { MONTACOCHES_CODIGO, MONTACOCHES_DESCRIPCION },
                { ELECTRICAS_CODIGO, ELECTRICAS_DESCRIPCION },
                { PASARELAS_RODANTES_CODIGO, PASARELAS_RODANTES_DESCRIPCION },
                { PANORAMICO_360_CODIGO, PANORAMICO_360_DESCRIPCION },
                { PANORAMICO_270_CODIGO, PANORAMICO_270_DESCRIPCION },
                { PANORAMICO_180_CODIGO, PANORAMICO_180_DESCRIPCION },
                { PANORAMICO_90_CODIGO, PANORAMICO_90_DESCRIPCION }
            };
        }

        public static class Sistema
        {
            public const string SIMPLEX_CODIGO = "Sim";
            public const string SIMPLEX_DESCRIPCION = "Simplex";
            public const string DUPLEX_CODIGO = "Dup";
            public const string DUPLEX_DESCRIPCION = "Duplex";
            public const string TRIPLEX_CODIGO = "Tri";
            public const string TRIPLEX_DESCRIPCION = "Triplex";
            public const string CUADRUPLEX_CODIGO = "Cua";
            public const string CUADRUPLEX_DESCRIPCION = "Cuadruplex";
            public const string QUINTUPLEX_CODIGO = "Qui";
            public const string QUINTUPLEX_DESCRIPCION = "Quintuplex";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { SIMPLEX_CODIGO, SIMPLEX_DESCRIPCION },
                { DUPLEX_CODIGO, DUPLEX_DESCRIPCION },
                { TRIPLEX_CODIGO, TRIPLEX_DESCRIPCION },
                { CUADRUPLEX_CODIGO, CUADRUPLEX_DESCRIPCION },
                { QUINTUPLEX_CODIGO, QUINTUPLEX_DESCRIPCION }
            };

            //public static 
        }

        public static class Marca
        {
            public const string ANDINO_CODIGO = "ANDI";
            public const string ANDINO_DESCRIPCION = "ANDINO";
            public const string AUTOMAC_CODIGO = "AUTO";
            public const string AUTOMAC_DESCRIPCION = "AUTOMAC";
            public const string CONTROLES_CODIGO = "COUR";
            public const string CONTROLES_DESCRIPCION = "CONTROLES S.A. (URU)";
            public const string DOPPLER_CODIGO = "DOPP";
            public const string DOPPLER_DESCRIPCION = "DOPPLER";
            public const string FUJI_CODIGO = "FUJI";
            public const string FUJI_DESCRIPCION = "FUJI";
            public const string FUJI_YIDA_CODIGO = "FUYI";
            public const string FUJI_YIDA_DESCRIPCION = "FUJI-YIDA";
            public const string HYUNDAI_CODIGO = "HYUN";
            public const string HYUNDAI_DESCRIPCION = "HYUNDAI";
            public const string IDE_CODIGO = "IDE";
            public const string IDE_DESCRIPCION = "IDE";
            public const string KONE_CODIGO = "KONE";
            public const string KONE_DESCRIPCION = "KONE";
            public const string LG_CODIGO = "LG";
            public const string LG_DESCRIPCION = "LG";
            public const string MITSUBISHI_CODIGO = "MITS";
            public const string MITSUBISHI_DESCRIPCION = "MITSUBISHI";
            public const string MOVILIFT_CODIGO = "MOVI";
            public const string MOVILIFT_DESCRIPCION = "MOVILIFT";
            public const string ORONA_CODIGO = "ORON";
            public const string ORONA_DESCRIPCION = "ORONA";
            public const string OTIS_CODIGO = "OTIS";
            public const string OTIS_DESCRIPCION = "OTIS";
            public const string SCHINDLER_CODIGO = "SCHI";
            public const string SCHINDLER_DESCRIPCION = "SCHINDLER";
            public const string SIGMA_CODIGO = "SIGM";
            public const string SIGMA_DESCRIPCION = "SIGMA";
            public const string TECMEIN_CODIGO = "TECM";
            public const string TECMEIN_DESCRIPCION = "TECMEIN";
            public const string TKE_THYSSENKRUPP_CODIGO = "TKET";
            public const string TKE_THYSSENKRUPP_DESCRIPCION = "TKE-THYSSENKRUPP";
            public const string OTROS_CODIGO = "OTRO";
            public const string OTROS_DESCRIPCION = "OTROS";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { ANDINO_CODIGO, ANDINO_DESCRIPCION },
                { AUTOMAC_CODIGO, AUTOMAC_DESCRIPCION },
                { CONTROLES_CODIGO, CONTROLES_DESCRIPCION },
                { DOPPLER_CODIGO, DOPPLER_DESCRIPCION },
                { FUJI_CODIGO, FUJI_DESCRIPCION },
                { FUJI_YIDA_CODIGO, FUJI_YIDA_DESCRIPCION },
                { HYUNDAI_CODIGO, HYUNDAI_DESCRIPCION },
                { IDE_CODIGO, IDE_DESCRIPCION },
                { KONE_CODIGO, KONE_DESCRIPCION },
                { LG_CODIGO, LG_DESCRIPCION },
                { MITSUBISHI_CODIGO, MITSUBISHI_DESCRIPCION },
                { MOVILIFT_CODIGO, MOVILIFT_DESCRIPCION },
                { ORONA_CODIGO, ORONA_DESCRIPCION },
                { OTIS_CODIGO, OTIS_DESCRIPCION },
                { SCHINDLER_CODIGO, SCHINDLER_DESCRIPCION },
                { SIGMA_CODIGO, SIGMA_DESCRIPCION },
                { TECMEIN_CODIGO, TECMEIN_DESCRIPCION },
                { TKE_THYSSENKRUPP_CODIGO, TKE_THYSSENKRUPP_DESCRIPCION },
                { OTROS_CODIGO, OTROS_DESCRIPCION }
            };
        }

        public static class SalaMaquinas
        {
            public const string MR_CODIGO = "MR";
            public const string MR_DESCRIPCION = "MR";
            public const string MRL_CODIGO = "MRL";
            public const string MRL_DESCRIPCION = "MRL";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { MR_CODIGO, MR_DESCRIPCION },
                { MRL_CODIGO, MRL_DESCRIPCION }
            };
        }

        public static class TipoMotor
        {
            public const string GEARLESS_CODIGO = "GEARLESS";
            public const string GEARLESS_DESCRIPCION = "GEARLESS";
            public const string GEARED_CODIGO = "GEARED";
            public const string GEARED_DESCRIPCION = "GEARED";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { GEARLESS_CODIGO, GEARLESS_DESCRIPCION },
                { GEARED_CODIGO, GEARED_DESCRIPCION }
            };
        }

        public static class Velocidad
        {
            public const string V050_CODIGO = "0.50";
            public const string V050_DESCRIPCION = "0.50";
            public const string V075_CODIGO = "0.75";
            public const string V075_DESCRIPCION = "0.75";
            public const string V100_CODIGO = "1.00";
            public const string V100_DESCRIPCION = "1.00";
            public const string V150_CODIGO = "1.50";
            public const string V150_DESCRIPCION = "1.50";
            public const string V175_CODIGO = "1.75";
            public const string V175_DESCRIPCION = "1.75";
            public const string V200_CODIGO = "2.00";
            public const string V200_DESCRIPCION = "2.00";
            public const string V250_CODIGO = "2.50";
            public const string V250_DESCRIPCION = "2.50";
            public const string V300_CODIGO = "3.00";
            public const string V300_DESCRIPCION = "3.00";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { V050_CODIGO, V050_DESCRIPCION },
                { V075_CODIGO, V075_DESCRIPCION },
                { V100_CODIGO, V100_DESCRIPCION },
                { V150_CODIGO, V150_DESCRIPCION },
                { V175_CODIGO, V175_DESCRIPCION },
                { V200_CODIGO, V200_DESCRIPCION },
                { V250_CODIGO, V250_DESCRIPCION },
                { V300_CODIGO, V300_DESCRIPCION }
            };
        }

        public static class TipoEmbarque
        {
            public const string SIMPLE_CODIGO = "Sim";
            public const string SIMPLE_DESCRIPCION = "Simple";
            public const string DOBLE_CODIGO = "Dob";
            public const string DOBLE_DESCRIPCION = "Doble";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { SIMPLE_CODIGO, SIMPLE_DESCRIPCION },
                { DOBLE_CODIGO, DOBLE_DESCRIPCION }
            };
        }

        public static class TipoDucto
        {
            public const string CONCRETO_CODIGO = "Sim";
            public const string CONCRETO_DESCRIPCION = "Concreto";
            public const string BLOQUE_CODIGO = "Dob";
            public const string BLOQUE_DESCRIPCION = "Bloque";
            public const string ESTRUCTURA_CODIGO = "Est"; // Corrected code for Estructura
            public const string ESTRUCTURA_DESCRIPCION = "Estructura";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { CONCRETO_CODIGO, CONCRETO_DESCRIPCION },
                { BLOQUE_CODIGO, BLOQUE_DESCRIPCION },
                { ESTRUCTURA_CODIGO, ESTRUCTURA_DESCRIPCION }
            };
        }

        public static class TipoEnergia
        {
            public const string BIFASICO_220_CODIGO = "Bi220";
            public const string BIFASICO_220_DESCRIPCION = "Bifasico 220";
            public const string TRIFASICO_220_CODIGO = "Tr220";
            public const string TRIFASICO_220_DESCRIPCION = "Trifasico 220";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { BIFASICO_220_CODIGO, BIFASICO_220_DESCRIPCION },
                { TRIFASICO_220_CODIGO, TRIFASICO_220_DESCRIPCION }
            };
        }

        public static class MaterialPuertas
        {
            public const string ACERO_INOXIDABLE_CODIGO = "AceIno";
            public const string ACERO_INOXIDABLE_DESCRIPCION = "Acero Inoxidable";
            public const string ACERO_PINTADO_CODIGO = "AcePint";
            public const string ACERO_PINTADO_DESCRIPCION = "Acero Pintado";
            public const string INOX_VIDRIO_CODIGO = "InoxVid";
            public const string INOX_VIDRIO_DESCRIPCION = "Acero Inox-Vidrio";

            public static Dictionary<string, string> ObtenerValores() => new Dictionary<string, string>
            {
                { ACERO_INOXIDABLE_CODIGO, ACERO_INOXIDABLE_DESCRIPCION },
                { ACERO_PINTADO_CODIGO, ACERO_PINTADO_DESCRIPCION },
                { INOX_VIDRIO_CODIGO, INOX_VIDRIO_DESCRIPCION }
            };
        }
    }
}
