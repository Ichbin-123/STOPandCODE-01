// Email Adresse: matteo@labforweb.academy


/* SysScol: Definisco il Range di voti di questo sistema scolastico fittizzio*/
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/** Docs: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types *//** https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types */
enum SysScol :byte { // Enum regola stilistica Pascal Case
    MinVoto = 0,
    Sufficiente = 6,
    MaxVoto = 10
}

/*ErrorCode: Definisco gli errori prodotti nella fase di controllo degli input dello user*/
enum ErrorCode :sbyte {    
    Successo = 0,
    ErroreVotoOutOfRange = -1,
    ErroreFormatoInput = -2,
    StringaNull = -3,
    NumeroNegativo = -4,
    DefaultError = -5
}

/*EsitoAnno: Definisco i risulati possibili di fine anno dello studente*/
enum EsitoAnno :sbyte {
    NonDefinito = -1,
    Bocciato = 0,
    Promosso = 1
};

/*StatisticheStudente: Definisco una classe pre gestire i risultati domandati dal Docente*/
class StatisticheStudente {
    private List<byte> listaVoti = new List<byte>(); // Lista voti studente
    private short sommaVoti =0;
    private float mediaVoti = 0f; 
    private byte votoMassimo = 0;
    private byte votoMinimo = 0;
    private byte votiSopraMedia = 0;
    private byte votiSottoMedia = 0;
    private byte votiInsufficienti = 0;
    private EsitoAnno risultato = EsitoAnno.NonDefinito;    
    //private int numeroVoti = 0;
    private class OggettoGrafico {
        private int[] origineAssi = new int[]{0, 0}; // Origine assi del Grafico
        private int[] coordinateStringaVoti = new int[0]; // Numero di voti presenti nella lista
        private int[] coordinateScalaVoti = new int[0]; // SysScol in questo esercizio vanno da 0 a 10
        private byte scalingFactor = 0;  // Fattore moltiplicativo asse voti
        private StatisticheStudente statEsterna;
        private List<byte> listaNote = new List<byte>();
        private int numeroVoti = 0;
        private int Xdisplacement = 0;
        private byte larghezzaBarraIstogramma = 0;
        private int lunghezzaAsseY = 0;
        private int lunghezzaAsseX = 0;
        private static class GraficaStatistica {
            /*Simboli*/
            public const char Solido =      '\u2588'; // VotoMassimo
            public const char Sfumato =     '\u2591'; // VotoSopraMedia
            public const char Sgranato =    '\u2592'; // VotoSottoMedia
            public const char Effimero =    '\u2593'; // VotoInsufficiente
            public const char Vuoto  =      '\u258F'; // VotoMinimo
            /*Colori*/
            public const ConsoleColor MediaVoti = ConsoleColor.Cyan;
            public const ConsoleColor VotoMassimo = ConsoleColor.Green;
            public const ConsoleColor VotoMinimo = ConsoleColor.DarkRed;
            public const ConsoleColor VotiSopraMedia = ConsoleColor.DarkGreen;
            public const ConsoleColor VotiSottoMedia = ConsoleColor.Red;
            public const ConsoleColor VotiInsufficienti = ConsoleColor.DarkYellow;
            public const ConsoleColor EsitoPromosso = ConsoleColor.Blue;
            public const ConsoleColor EsitoBocciato = ConsoleColor.Red;
        }
        public OggettoGrafico(int x, int y, StatisticheStudente statistiche, byte fattoreMoltiplicativo, byte widthBars) {
            this.statEsterna = statistiche;
            this.listaNote= this.statEsterna.listaVoti;
            this.numeroVoti = this.listaNote.Count;
            this.Xdisplacement = ((int)"voto ".Length + (int)Convert.ToString(this.numeroVoti).Length + (int)"  ".Length);
            this.scalingFactor = fattoreMoltiplicativo;
            this.larghezzaBarraIstogramma = widthBars;
            this.lunghezzaAsseY = ((SysScol.MaxVoto -SysScol.MinVoto)+1)*(this.scalingFactor);
            this.lunghezzaAsseX = (this.larghezzaBarraIstogramma +1)*(this.numeroVoti);
            InizializzaOrigineAssi(this.Xdisplacement, y+1);
            InizializzaCoordinateLabels(this.numeroVoti);
        } /*Costruttore OggettoGrafico*/
        private void InizializzaOrigineAssi(int x, int y){
            origineAssi[0] = x;
            origineAssi[1] = y;
        } /*InizializzaOrigineAssi*/        
        private void InizializzaCoordinateLabels(int numeroVoti){
            int scala = (int)(SysScol.MaxVoto+1);
            this.coordinateStringaVoti = new int[numeroVoti]; 
            this.coordinateScalaVoti = new int [scala];
            for(int i=0; i<scala; i++){
                this.coordinateScalaVoti[i]= this.origineAssi[0] + (i * this.scalingFactor);
            }
            for(int i=0; i<numeroVoti; i++){
                this.coordinateStringaVoti[i]= this.origineAssi[1]+2 + (i*this.larghezzaBarraIstogramma);
            }
        }/*InizializzaCoordinateLabels*/
        
        public void TracciaAssi(){
            Console.CursorVisible = false;
            Console.SetBufferSize(Console.WindowWidth, this.lunghezzaAsseY+ 10); // Aumenta il Buffer Console

            /*Label asse Y*/
            for(int i=0; i<coordinateScalaVoti.Length; i++){
                Console.SetCursorPosition(this.coordinateScalaVoti[i], this.origineAssi[1]-1);
                Console.Write(i);
                Thread.Sleep(5);
            }
            /*Asse Y*/
            for(int i=0; i<this.lunghezzaAsseY; i++){
                Console.SetCursorPosition(this.origineAssi[0]+i, this.origineAssi[1]);
                Console.Write("-");
                if(i==this.lunghezzaAsseY-1) {Console.WriteLine(">");}                
                Thread.Sleep(5);
            }
            
            int valoreIntero = (int)Math.Floor(statEsterna.mediaVoti);
            int parteConLaVirgola = (int)Math.Truncate((statEsterna.mediaVoti - valoreIntero)*100);
            int proporzioneAggiunta = (int)Math.Round((double)((this.scalingFactor*parteConLaVirgola)/100));
            int valoreScalato = (int)((valoreIntero * this.scalingFactor)+proporzioneAggiunta+this.Xdisplacement);
            int lunghezzaMedia = (int)(this.lunghezzaAsseX - Math.Ceiling((double)this.scalingFactor/2));
            /*Media*/            
            Console.ForegroundColor = ConsoleColor.Cyan;
            for(int i=0; i<lunghezzaMedia; i++){
                Console.SetCursorPosition((int)valoreScalato, this.origineAssi[1]+1+i);
                Console.WriteLine("|");
            }            
            Console.ResetColor();

            /*Label asse X - Asse X - Istogramma*/
            for(int i=0; i<this.lunghezzaAsseX; i++){
                for(int j=0; j<this.coordinateStringaVoti.Length; j++){
                    Thread.Sleep(5);
                    /*Label asse X*/
                    if(this.coordinateStringaVoti[j]==this.origineAssi[1]+i){
                        Console.SetCursorPosition(1, (int)this.coordinateStringaVoti[j]);
                        Console.WriteLine($"voto {j+1}");
                        int barraIstogramma = this.listaNote[j] * this.scalingFactor;
                        /*Istogramma*/
                        for(int k=0; k<barraIstogramma; k++){
                            Thread.Sleep(5);
                            Console.SetCursorPosition(this.Xdisplacement+k+1, (int)this.coordinateStringaVoti[j]);
                            Console.Write((char)GraficaStatistica.Solido);
                        } // for k
                    } // if                    
                } // for j
                /*Asse X*/
                Console.SetCursorPosition(this.Xdisplacement, this.origineAssi[1]+i);
                Console.WriteLine("|");
                if(i==this.lunghezzaAsseX-1){
                    Console.SetCursorPosition(this.Xdisplacement, this.origineAssi[1]+i);
                    Console.WriteLine("v");
                    }
                Thread.Sleep(5);
            } // for i
        } /*Traccia Assi*/
        

    } /*OggettoGrafico*/


    [Flags]
    enum StatoStatistiche :byte {
        Nessuno                     = 0,        // 0 > 00000000 < Reset
        SommaVotiChecker            = 1 << 0,   // 1 > 00000001
        MediaVotiChecker            = 1 << 1,   // 2 > 00000010
        VotoMassimoChecker          = 1 << 2,   // 4 > 00000100
        VotoMinimoChecker           = 1 << 3,   // 8 > 00001000
        VotiSopraMediaChecker       = 1 << 4,   // 16 > 00010000
        VotiSottoMediaChecker       = 1 << 5,   // 32 > 00100000
        VotiInsufficientiChecker    = 1 << 6,   // 64 > 01000000
        RisultatoChecker            = 1 << 7    // 128 > 10000000
    }

    StatoStatistiche StatoCalcoliStatistici = StatoStatistiche.Nessuno; // Metti i flag tutti a zero

    public void StampaVotiStudente(){        
        Console.WriteLine($"Voti inseriti: [{string.Join(", ",listaVoti)}]");
    }
    public void MostraStatistiche(StatisticheStudente statistiche){
        CalcolaStatistiche();
        
        Console.WriteLine("\n");
        Console.WriteLine("--- RISULTATI ---");
        Console.WriteLine($"Voti inseriti: [{string.Join(", ",listaVoti)}]");
        Console.WriteLine($"Somma voti: {sommaVoti}");
        Console.WriteLine($"Media voti:  {mediaVoti:F2}");
        Console.WriteLine($"Voto massimo:  {votoMassimo}");
        Console.WriteLine($"Voto minimo: {votoMinimo}");
        Console.WriteLine($"Voti sopra la media: {votiSopraMedia}");
        Console.WriteLine($"Voti sotto la media: {votiSottoMedia}");
        Console.WriteLine($"Voti insufficienti: {votiInsufficienti}");
        Console.WriteLine($"Risultato: {risultato}");
        Console.WriteLine("-----------------");
        Console.WriteLine("\n");
        Console.ReadLine();
        Console.Clear();
        OggettoGrafico oggettoGrafico = new OggettoGrafico(Console.CursorLeft, Console.CursorTop, statistiche, 7, 3);
        oggettoGrafico.TracciaAssi();        
    }

    public ErrorCode InserisciVoto(string? input){
        if(input==""){
            return ErrorCode.StringaNull;
        }
        if(byte.TryParse(input, out byte voto)){
            if(voto>= (byte)SysScol.MinVoto && voto<= (byte)SysScol.MaxVoto){
                listaVoti.Add(voto);
                StatoCalcoliStatistici = StatoStatistiche.Nessuno; // Invalida statistiche precedenti
                return ErrorCode.Successo;
            } else {
                return ErrorCode.ErroreVotoOutOfRange;
            }
        } /*if TryParse()*/
        return ErrorCode.ErroreFormatoInput;
    } /*InserisciVoto*/

    private void CalcolaSommaVoti(){
        sommaVoti = 0;
        foreach (short voto in listaVoti){
            sommaVoti+=voto;
        }
        StatoCalcoliStatistici |= StatoStatistiche.SommaVotiChecker; // Aggiorna Flag somma
    } /*CalcolaSommaVoti*/

    private void CalcolaMediaVoti(){
        if(listaVoti.Count==0){ // Evita divisione per zero
            mediaVoti=0;
        } else {
            if((StatoCalcoliStatistici & StatoStatistiche.SommaVotiChecker)==0){
                CalcolaSommaVoti();
            }
            mediaVoti = (float)sommaVoti/listaVoti.Count;
        }
        StatoCalcoliStatistici |= StatoStatistiche.MediaVotiChecker; // Aggiorna Flag media
        // Console.WriteLine($"Sommavoti: {sommaVoti}, Listavoticount: {listaVoti.Count}, Media: {mediaVoti}");
    } /*CalcolaMediaVoti*/

    private void CalcolaVotoMassimo(){
        votoMassimo=0;
        foreach (byte voto in listaVoti){
            if(votoMassimo<voto){ votoMassimo = voto; }
        }
        StatoCalcoliStatistici |= StatoStatistiche.VotoMassimoChecker; // Aggiorna Flag voto massimo
    } /*CalcolaVotoMassimo*/

    private void CalcolaVotoMinimo(){
        if(listaVoti.Count==0){     // Quando la lista è vuota votoMinimo deve essere = 0
            votoMinimo=0;
        } else {
            votoMinimo= 10;
        }
        foreach (byte voto in listaVoti){
            if(votoMinimo>voto){ votoMinimo = voto; }
        }
        StatoCalcoliStatistici |= StatoStatistiche.VotoMinimoChecker; // Aggiorna Flag voto minimo
    } /*CalcolaVotoMinimo*/

    private void CalcolaVotiSopraMedia(){
        if((StatoCalcoliStatistici & StatoStatistiche.MediaVotiChecker)==0){
            CalcolaMediaVoti();
        }
        votiSopraMedia = 0;
        foreach (byte voto in listaVoti){
            if(mediaVoti<voto) {votiSopraMedia++;}
        }
        StatoCalcoliStatistici |= StatoStatistiche.VotiSopraMediaChecker; // Aggiorna Flag voti sopra media
    } /*CalcolavotiSopraMedia*/
    
    private void CalcolaVotiSottoMedia(){
        if((StatoCalcoliStatistici & StatoStatistiche.MediaVotiChecker)==0){
            CalcolaMediaVoti();
        }
        votiSottoMedia=0;
        foreach (byte voto in listaVoti){
            if(mediaVoti>voto) {votiSottoMedia++;}
        }
        StatoCalcoliStatistici |= StatoStatistiche.VotiSottoMediaChecker; // Aggiorna Flag voti sotto media
    } /*CalcolaVotiSottoMedia*/

    private void CalcolaVotiInsufficienti(){
        votiInsufficienti=0;
        foreach (byte voto in listaVoti){
            if(voto<(byte)SysScol.Sufficiente) {votiInsufficienti++;}
        }
        StatoCalcoliStatistici |= StatoStatistiche.VotiInsufficientiChecker; // Aggiorna Flag voti insufficienti
    } /*CalcolaVotiInsufficienti*/

    private void CalcolaRisultato(){
        if((StatoCalcoliStatistici & StatoStatistiche.MediaVotiChecker)==0){
            CalcolaMediaVoti();
        }
        if(mediaVoti>=6){risultato = EsitoAnno.Promosso;
        } else {
            risultato = EsitoAnno.Bocciato;
        }
        StatoCalcoliStatistici |= StatoStatistiche.RisultatoChecker; // Aggiorna Flag risultato anno scolastico
    } /*CalcolaRisultato*/

    private void CalcolaStatistiche(){
        if((StatoCalcoliStatistici & StatoStatistiche.SommaVotiChecker)==0){
            CalcolaSommaVoti();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.MediaVotiChecker)==0){
            CalcolaMediaVoti();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.VotoMassimoChecker)==0){
            CalcolaVotoMassimo();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.VotoMinimoChecker)==0){
            CalcolaVotoMinimo();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.VotiSopraMediaChecker)==0){
            CalcolaVotiSopraMedia();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.VotiSottoMediaChecker)==0){
            CalcolaVotiSottoMedia();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.VotiInsufficientiChecker)==0){
            CalcolaVotiInsufficienti();
        }
        if((StatoCalcoliStatistici & StatoStatistiche.RisultatoChecker)==0){
            CalcolaRisultato();
        }
    } /*CalcolaStatistiche*/


};


class Program {

    public static void StampaMessaggioErrore(ErrorCode code){
        switch(code){
            case ErrorCode.Successo: // ErrorCode = 0
                Console.WriteLine("Input Accettato!");
                break;
            case ErrorCode.ErroreVotoOutOfRange: // ErrorCode = -1
                Console.WriteLine("Errore: Il voto deve avere un valore compreso tra 0 e 10 inclusi [0, 10]!");
                break;
            case ErrorCode.ErroreFormatoInput: // ErrorCode = -2
                Console.WriteLine("Errore: Formato input non valido. Inserrisci un numero [0-9]+!");
                break;
            case ErrorCode.StringaNull: // ErrorCode = -3
                Console.WriteLine("Errore: L'input non può essere una stringa vuota!");
                break;
            case ErrorCode.NumeroNegativo: // ErrorCode = -4
                Console.WriteLine("Errore: L'input non può essere un numero negativo!");
                break;
            case ErrorCode.DefaultError: // ErrorCode = -5
            Console.WriteLine("Errore: Default Error!");
            break;
            default:
                Console.WriteLine("Errore Sconosciuto!!!");
                break;
        }
    } /*StampaMessaggioErrore*/

    public static ErrorCode TestaStringa(string? inputUser){

        if(inputUser==""){
            return ErrorCode.StringaNull;
        }
        if(!byte.TryParse(inputUser, out byte numeroTentativi)){
            if(int.TryParse(inputUser, out int numeroTentativi2)){
                if(numeroTentativi<0){
                    return ErrorCode.NumeroNegativo;
                    }
            }
            return ErrorCode.ErroreFormatoInput;
        }
        return ErrorCode.Successo;
    } /*TestaStringa*/
    
    static void Main(string[] args){
        StatisticheStudente studente = new StatisticheStudente();
        byte numeroVoti=0;
        ErrorCode controlloInput = ErrorCode.DefaultError;
        Console.WriteLine("\n");
        while(controlloInput!=ErrorCode.Successo){
            Console.Write("Quanti voti vuoi inserire? ");
            string? inputQuantita = Console.ReadLine();
            controlloInput = TestaStringa(inputQuantita);
            StampaMessaggioErrore(controlloInput);
            if(controlloInput==ErrorCode.Successo){
                numeroVoti = Convert.ToByte(inputQuantita);
            }
        } /*while(controlloInput!=*/

        while(numeroVoti>0){
            Console.Write("Inserisci un voto [0-10]: ");
            string? inputVoto  = Console.ReadLine();
            ErrorCode esitoInserzione = studente.InserisciVoto(inputVoto);
            if(esitoInserzione==ErrorCode.Successo){                
                numeroVoti--;
            } else {
                StampaMessaggioErrore(esitoInserzione);
            }
        } /*while(numeroVoti>0)*/

        studente.MostraStatistiche(studente);
        Console.ReadLine();
    } /*Main*/
} /*Program*/