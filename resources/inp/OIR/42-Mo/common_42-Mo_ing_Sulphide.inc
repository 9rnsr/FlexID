[include:common_42-Mo_parameter.inc]

[parameter]
  # ICRP Publ.134 p.305 Table 14.2 / Ingested material, Sulphide

$if parent
  ${Mo}/fA    = 0.05
  ${Mo}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
[include:element_42-Mo.inc]
[include:element_41-Nb.inc]
