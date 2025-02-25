using System.Diagnostics;
using System.IO;
using System.Text;

namespace CryptoSoft;

/// <summary>
/// Classe permettant de gérer le chiffrement et le déchiffrement des fichiers.
/// </summary>
public class GestionnaireFichier
{
    private string CheminFichier { get; }
    private string CheminFichierChiffre { get; }
    private string Cle { get; }

    public GestionnaireFichier(string cheminFichier, string cle)
    {
        CheminFichier = cheminFichier;
        Cle = cle;
        string repertoire = Path.GetDirectoryName(cheminFichier) ?? "";
        string nomSansExtension = Path.GetFileNameWithoutExtension(cheminFichier);
        string extension = Path.GetExtension(cheminFichier);
        CheminFichierChiffre = Path.Combine(repertoire, $"{nomSansExtension}{extension}");
    }

    /// <summary>
    /// Vérifie si le fichier existe.
    /// </summary>
    private bool VerifierFichier()
    {

        // Vérifie si le fichier existe
        if (File.Exists(CheminFichier))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Effectue le chiffrement ou déchiffrement du fichier avec l'algorithme XOR.
    /// </summary>
    public int TransformerFichier()
    {
        if (!VerifierFichier()) return -1;

        Stopwatch chronometre = Stopwatch.StartNew();
        var octetsFichier = File.ReadAllBytes(CheminFichier);
        var octetsCle = ConvertirEnOctets(Cle);
        var octetsTransformes = MethodeXor(octetsFichier, octetsCle);
        File.WriteAllBytes(CheminFichierChiffre, octetsTransformes);
        chronometre.Stop();

        return (int)chronometre.ElapsedMilliseconds;
    }

    /// <summary>
    /// Convertit une chaîne de caractères en tableau d'octets.
    /// </summary>
    private static byte[] ConvertirEnOctets(string texte)
    {
        return Encoding.UTF8.GetBytes(texte);
    }

    /// <summary>
    /// Applique l'algorithme XOR sur un fichier.
    /// </summary>
    private static byte[] MethodeXor(IReadOnlyList<byte> octetsFichier, IReadOnlyList<byte> octetsCle)
    {
        var resultat = new byte[octetsFichier.Count];
        for (var i = 0; i < octetsFichier.Count; i++)
        {
            resultat[i] = (byte)(octetsFichier[i] ^ octetsCle[i % octetsCle.Count]);
        }
        return resultat;
    }
}