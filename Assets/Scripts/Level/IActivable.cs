using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivable {

    /// <summary>
    /// L'objet peut être activé par le cannibal
    /// </summary>
    bool IsActivable(CannibalObject cannibal);

    /// <summary>
    /// Active l'objet et consomme l'objet tenu par le cannibal si besoin
    /// </summary>
    void Activate(CannibalObject cannibal);

    void ShowIcon();

}
