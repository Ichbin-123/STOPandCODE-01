// Email Adresse: matteo@labforweb.accademy

/** Docs: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types *//** https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types */
/* SysScol: Definisco il Range di voti di questo sistema scolastico fittizzio*/
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
    public void MostraStatistiche(){
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
            mediaVoti = sommaVoti/listaVoti.Count;
        }
        StatoCalcoliStatistici |= StatoStatistiche.MediaVotiChecker; // Aggiorna Flag media
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

        studente.MostraStatistiche();
    } /*Main*/
} /*Program*/