<?php

$action = $_POST['action'] ;
$tab_result [] = array() ;
$conn = mysqli_connect("localhost","root","","gsb");

function connecterServeurBD() {
    $hote = "localhost";
    $login = "root";
    $mdp = "";
    return mysqli_connect($hote,$login,$mdp,"gsb"); 
}
 
  // Parcours du tableau
if($action == "connect_comptable"){
    
    $login = $_POST['Identifiant'] ;
    $mdp = $_POST['Mot_de_passe'] ;
    $sql = "SELECT Id_Comptable FROM Comptable WHERE login =  '" .$login ."' AND mdp = '".$mdp."'"  ;
    $resultat = $conn->query($sql);
    
    if(!empty($resultat)){
      header('Content-type: application/json');
      echo json_encode("Ok",JSON_UNESCAPED_UNICODE);
    }
    else{
      header('Content-type: application/json');
      echo json_encode("Ko",JSON_UNESCAPED_UNICODE);
    }
}

if($action == "Recup_Nom_Visiteur"){
    
    $sql = "SELECT distinct nom,id FROM FicheFrais INNER JOIN Visiteur on id = Visiteur.id WHERE (idEtat = 'CR' AND id IN (SELECT idVisiteur FROM FicheFrais))" ; 
    $resultat = connecterServeurBD()->query($sql);
    
    if(!empty($resultat)){
      $res = $resultat->fetch_all(MYSQLI_ASSOC) ;
      foreach ($res as $ligne){
          $resutf8 [] = array(
              'id' => utf8_encode($ligne['id']),
              'nom' => utf8_encode($ligne['nom']));
      }
      header('Content-type: application/json');
      echo json_encode($resutf8,JSON_UNESCAPED_UNICODE);
    }
    else{
      header('Content-type: application/json');
      echo json_encode("Ko",JSON_UNESCAPED_UNICODE);
    }
}

if($action == "Recup_Nom_Visiteur_Fiche_Valide"){
    
    $sql = "SELECT distinct nom,id FROM FicheFrais INNER JOIN Visiteur on id = Visiteur.id WHERE (id IN (SELECT idVisiteur FROM FicheFrais) AND (idEtat = 'VA' OR idEtat ='RB'))" ; 
    $resultat = connecterServeurBD()->query($sql);
    
    if(!empty($resultat)){
      $res = $resultat->fetch_all(MYSQLI_ASSOC) ;
      foreach ($res as $ligne){
          $resutf8 [] = array(
              'id' => utf8_encode($ligne['id']),
              'nom' => utf8_encode($ligne['nom']));
      }
      header('Content-type: application/json');
      echo json_encode($resutf8,JSON_UNESCAPED_UNICODE);
    }
    else{
      header('Content-type: application/json');
      echo json_encode("Ko",JSON_UNESCAPED_UNICODE);
    }
}

if(isset($_POST['action']))
{
    if($action == "Recup_Frais_Fortfait")
    {
        $id = $_POST['id'] ; 
        $Date = $_POST['Date'] ; 
        

        $sql = "SELECT quantite FROM LigneFraisForfait WHERE idVisiteur = '" .$id ."' AND mois = '".$Date."'" ;
        if($result = $conn->query($sql))
        {
                    
            $verif = $result->num_rows ;  
            if($verif != 0)
            {
                $i = 0 ;
                while ($row  = $result->fetch_assoc())
                {
                    $tab_result[$i][0] = $row["quantite"] ; 
                    $i++ ; 
                }
                header('Content-type: application/json');
                echo json_encode($tab_result,JSON_UNESCAPED_UNICODE);
            }
            else
            {
                header('Content-type: application/json');
                echo json_encode("Ko",JSON_UNESCAPED_UNICODE);  
            }
        }
    }    
}    
    if(isset($_POST['action']))
{
    if($action == "Recup_Mois_Fiche")
    {
        $id = $_POST['id'] ; 

        $sql = "SELECT mois FROM FicheFrais WHERE idVisiteur = '".$id."'";
        if($result = $conn->query($sql))
        {
                     
            $verif = $result->num_rows ;  
            if($verif != 0)
            {
                $i = 0 ;
                while ($row  = $result->fetch_assoc())
                {
                    $tab_result[$i][0] = $row["mois"] ; 
                    $i++ ; 
                }
                header('Content-type: application/json');
                echo json_encode($tab_result,JSON_UNESCAPED_UNICODE);
            }
            else
            {
                header('Content-type: application/json');
                echo json_encode("Ko",JSON_UNESCAPED_UNICODE);  
            }
        }
    }    
    
}

    if(isset($_POST['action']))
{
    if($action == "Recup_Mois_Fiche_Valide")
    {
        $id = $_POST['id'] ; 

        $sql = "SELECT mois FROM FicheFrais WHERE idVisiteur = '".$id."' AND idEtat = 'VA'" ;
        if($result = $conn->query($sql))
        {
                     
            $verif = $result->num_rows ;  
            if($verif != 0)
            {
                $i = 0 ;
                while ($row  = $result->fetch_assoc())
                {
                    $tab_result[$i][0] = $row["mois"] ; 
                    $i++ ; 
                }
                header('Content-type: application/json');
                echo json_encode($tab_result,JSON_UNESCAPED_UNICODE);
            }
            else
            {
                header('Content-type: application/json');
                echo json_encode("Ko",JSON_UNESCAPED_UNICODE);  
            }
        }
    }    
    
}
if(isset($_POST["action"]))
{
    if($action == "MAJ_Frais_Forfait")
    {
        $ETP = $_POST["ETP"] ; 
        $KM = $_POST["KM"] ; 
        $NUI = $_POST["NUI"] ; 
        $REP = $_POST["REP"] ; 
        $id = $_POST["id"] ; 
        $mois = $_POST["mois"] ;
        
        $sql = "UPDATE LigneFraisForfait SET quantite = '".$ETP."' WHERE (idVisiteur = '".$id."' AND mois = '".$mois."' AND idFraisForfait = 'ETP')" ; 
        $conn->query($sql) ;
        $sql1 = "UPDATE LigneFraisForfait SET quantite = '".$KM."' WHERE idVisiteur = '".$id."' AND mois = '".$mois."' AND idFraisForfait = 'KM'" ; 
        $conn->query($sql1) ; 
        $sql2 = "UPDATE LigneFraisForfait SET quantite = '".$NUI."' WHERE idVisiteur = '".$id."' AND mois = '".$mois."' AND idFraisForfait = 'NUI'" ; 
        $conn->query($sql2) ; 
        $sql3 = "UPDATE LigneFraisForfait SET quantite = '".$REP."' WHERE idVisiteur = '".$id."' AND mois = '".$mois."' AND idFraisForfait = 'REP'" ; 
        $conn->query($sql3) ; 
        
    }
    
}

if(isset($_POST["action"]))
{
    if($action == "Recup_Frais_Hors_Fortfait")
    {
        $id = $_POST["id"] ; 
        $mois = $_POST["mois"] ;
        
        $sql = "SELECT montant,libelle,date FROM LigneFraisHorsForfait WHERE (idVisiteur = '" .$id ."' AND mois = '".$mois."' AND etat = 0)"   ;
        
        if($result = $conn->query($sql))
        {
                    
            $verif = $result->num_rows ;  
            if($verif != 0)
            {
                $i = 0 ;
                while ($row  = $result->fetch_assoc())
                {
                    $tab_result[$i][0] = $row["montant"] ; 
                    $tab_result[$i][1] = $row["libelle"] ; 
                    $tab_result[$i][2] = $row["date"] ; 
                    
                    $i++ ; 
                }
                header('Content-type: application/json');
                echo json_encode($tab_result,JSON_UNESCAPED_UNICODE);
            }
            else
            {
                header('Content-type: application/json');
                echo json_encode("Ko",JSON_UNESCAPED_UNICODE);  
            }
        }
        
    }
}

if(isset($_POST["action"]))
{
    if($action == "MAJ_Frais_Hors_Forfait")
    {
        $libelle = $_POST["libelle"] ; 
        $id = $_POST["id"] ; 
        $mois = $_POST["mois"] ;
        
        $sql = "UPDATE LigneFraisHorsForfait SET etat = 1, libelle = CONCAT('REFUSE : ',libelle) WHERE (idVisiteur = '$id' AND mois = '$mois' AND libelle = '$libelle')" ; 
        $result =  $conn->query($sql) ; 
    }
}
if(isset($_POST["action"]))
{
    if($action == "MAJ_Etat_Fiche")
    {
        $id = $_POST["id"] ; 
        $mois = $_POST["mois"] ;
        $total = $_POST["total"] ;    
        
        $sql = "UPDATE FicheFrais SET idEtat = 'VA', montantValide = '$total' WHERE (idVisiteur = '$id' AND mois = '$mois')" ; 
        $result =  $conn->query($sql) ; 
    }
}

    
     
         
        


    

