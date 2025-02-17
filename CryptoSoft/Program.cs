namespace CryptoSoft;

public class CryptoService
{
    // Point d'entrée du programme, méthode 'Main' statique acceptant les arguments de ligne de commande
    public static void Main(string[] args)
    {
        // Vérification des arguments
        if (args.Length < 2)
        {
            return;
        }

        string cheminFichier = args[0];
        string cle = args[1];

        // Appel de la méthode Transformer avec les arguments passés
        Transformer(cheminFichier, cle);
    }

    // Méthode pour effectuer la transformation du fichier
    public static void Transformer(string cheminFichier, string cle)
    {
        try
        {
            var gestionnaireFichier = new GestionnaireFichier(cheminFichier, cle);
            int tempsEcoule = gestionnaireFichier.TransformerFichier();
        }
        catch (Exception e)
        {
        }
    }
}
