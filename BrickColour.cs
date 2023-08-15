using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;


namespace LegoSocInstaPostMaker {
    class BrickColour {
        //rnd generator
        static Random rnd = new Random();

        #region Colours
        //list of common colours
        public static Brush Black           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#05131D");
        public static Brush White           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
        public static Brush LightBluishGray { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#A0A5A9");
        public static Brush DarkBluishGray  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#6C6E68");
        public static Brush Red             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#C91A09");
        public static Brush Yellow          { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F2CD37");
        public static Brush Blue            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#0055BF");
        public static Brush ReddishBrown    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#582A12");
        public static Brush Tan             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#E4CD9E");
        public static Brush Green           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#237841");
        public static Brush Orange          { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FE8A18");
        #endregion

        #region Expanded Colours // TODO: remove inactive colours
        
        //public static Brush White                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F4F4F4");
        public static Brush Grey                   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#8A928D");
        public static Brush LightYellow            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD67F");
        public static Brush BrickRed               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F2705E");
        public static Brush BrickYellow            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#B0A06F");
        public static Brush LightGreen             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#ADD9A8");
      //public static Brush Orange                 { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8500");
        public static Brush CobaltBlue             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#8C00FF");
        public static Brush LightReddishViolet     { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F6A9BB");
        public static Brush PastelBlue             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#ABD9FF");
        public static Brush LightOrangeBrown       { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#D86D2C");
        public static Brush RedOrange              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8014");
        public static Brush PastelGreen            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#78FC78");
        public static Brush Lemon                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF230");
        public static Brush Pink                   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF879C");
        public static Brush Rose                   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF9494");
        public static Brush Nougat                 { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#BB805A");
        public static Brush LightBrown             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#CF8A47");
        public static Brush BrightRed              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#B40000");
        public static Brush MediumReddishViolet    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#D05098");
        public static Brush BrightBlue             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#1E5AA8");
        public static Brush BrightYellow           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FAC80A");
        public static Brush EarthOrange            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#543324");
      //public static Brush Black                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#1B2A34");
        public static Brush DarkGrey               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#545955");
        public static Brush DarkGreen              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#00852B");
        public static Brush MediumGreen            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#7FC475");
        public static Brush LightYellowishOrange   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FDC383");
        public static Brush BrightGreen            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#58AB41");
        public static Brush DarkOrange             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#91501C");
        public static Brush LightBluishViolet      { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#AFBED6");
        public static Brush LightBlue              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#97CBD9");
        public static Brush LightRed               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F9B7A5");
        public static Brush MediumRed              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F06D61");
        public static Brush MediumBlue             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#7396C8");
        public static Brush LightGrey              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#BCB4A5");
        public static Brush BrightViolet           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#671FA1");
        public static Brush BrightYellowishOrange  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F58624");
        public static Brush BrightOrange           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#D67923");
        public static Brush BrightBluishGreen      { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#069D9F");
        public static Brush EarthYellow            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#56472F");
        public static Brush BrightBluishViolet     { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#26469A");
        public static Brush MediumBluishViolet     { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#4861AC");
        public static Brush MediumYellowishGreen   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#B7D425");
        public static Brush MediumBluishGreen      { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#00AAA4");
        public static Brush LightBluishGreen       { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#9CD6CC");
        public static Brush BrightYellowishGreen   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#A5CA18");
        public static Brush LightYellowishGreen    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#DEEA92");
        public static Brush MediumYellowishOrange  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F89A39");
        public static Brush BrightReddishOrange    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#EE5434");
        public static Brush BrightReddishViolet    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#901F76");
        public static Brush LightOrange            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F9A777");
        public static Brush DarkNougat             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#AD6140");
        public static Brush NeonOrange             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#EF5828");
        public static Brush NeonGreen              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#CDDD34");
        public static Brush SandBlue               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#70819A");
        public static Brush SandViolet             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#75657D");
        public static Brush MediumOrange           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F48147");
        public static Brush SandYellow             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#897D62");
        public static Brush EarthBlue              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#19325A");
        public static Brush EarthGreen             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#00451A");
        public static Brush DarkArmyGreen          { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#887F4A");
        public static Brush SandGreen              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#708E7C");
        public static Brush SandRed                { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#88605E");
        public static Brush DarkRed                { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#720012");
        public static Brush Curry                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#DD982E");
        public static Brush TinyBlue               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#009ECE");
        public static Brush FireYellow             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCF0B");
        public static Brush FlameYellowishOrange   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCAC00");
      //public static Brush ReddishBrown           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#5F3109");
        public static Brush FlameReddishOrange     { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#EC441D");
        public static Brush MediumStoneGrey        { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#969696");
        public static Brush RoyalBlue              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#1C58A7");
        public static Brush DarkRoyalBlue          { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#0E3E9A");
        public static Brush BrightLilac            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#312B87");
        public static Brush BrightReddishLilac     { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#8A12A8");
        public static Brush DarkStoneGrey          { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#646464");
        public static Brush LightStoneGrey         { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#C8C8C8");
        public static Brush DarkCurry              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#A47624");
        public static Brush FadedGreen             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#468A5F");
        public static Brush Turquoise              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#3FB6A9");
        public static Brush LightRoyalBlue         { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#9DC3F7");
        public static Brush MediumRoyalBlue        { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#476FB6");
        public static Brush Rust                   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#872B17");
        public static Brush Brown                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#7B5D41");
        public static Brush ReddishLilac           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#8E5597");
        public static Brush Lilac                  { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#564E9D");
        public static Brush LightLilac             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#9195CA");
        public static Brush BrightPurple           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#D3359D");
        public static Brush LightPurple            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF9ECD");
        public static Brush LightPink              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F17880");
        public static Brush LightBrickYellow       { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#F3C988");
        public static Brush WarmYellowishOrange    { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FAA964");
        public static Brush CoolYellow             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFEC6C");
        public static Brush DoveBlue               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#77C9D8");
        public static Brush LightFadedGreen        { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#60BA76");
        public static Brush MediumLilac            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#441A91");
        public static Brush TinyMediumBlue         { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#3E95B6");
        public static Brush LightNougat            { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC995");
        public static Brush FlamingoPink           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF94C2");
        public static Brush DarkBrown              { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#372100");
        public static Brush MediumNougat           { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#AA7D55");
        public static Brush DarkAzur               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#469BC3");
        public static Brush MediumAzur             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#68C3E2");
        public static Brush Aqua                   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#D3F2EA");
        public static Brush MediumLavender         { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#A06EB9");
        public static Brush Lavender               { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#CDA4DE");
        public static Brush SpringYellowishGreen   { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#E2F99A");
        public static Brush OliveGreen             { get; } = (SolidColorBrush)new BrushConverter().ConvertFrom("#77774E");
        /**/
        #endregion


        /// <summary>
        /// Picks a random brick colour.
        /// </summary>
        /// <returns> A brush with a random brick colour.</returns>
        public static Brush GetRandom() {
            //gets all public static properties
            PropertyInfo[] colours = typeof(BrickColour).GetProperties(BindingFlags.Static | BindingFlags.Public);

            //selects a random one
            int r = rnd.Next(1, colours.Length) - 1;

            //returns it
            return (Brush)colours[r].GetValue(null);

        }

        /// <summary>
        /// Gets the corresponding name of a provided brush colour.
        /// </summary>
        /// <param name="brush"> A Brush (castable to SolidColourBrush) that has an entry in this class. </param>
        /// <returns> The name of the provided brick colour. Returns <see langword="null"/> if not found. </returns>
        public static string GetName(Brush brush) {
            //gets all public static properties
            PropertyInfo[] colours = typeof(BrickColour).GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (PropertyInfo p in colours) {
                //if the brush colour is the same as a value colour
                if (((SolidColorBrush)p.GetValue(null)).Color == ((SolidColorBrush)brush).Color ) {
                    return p.Name;
                }
            }

            //no corresponding value found
            return null;
        }

        /// <summary>
        /// Gets the brush of the colour of the provided name.
        /// </summary>
        /// <param name="name"> The name of the brick colour. </param>
        /// <returns> The brush with the appropriate colour. Returns <see langword="null"/> if not found. </returns>
        public static Brush FromName(string name) {
            try { 
                return (Brush)typeof(BrickColour).GetProperty(name).GetValue(null);
            }
            catch {
                return null;
            }
        }
    }
}
