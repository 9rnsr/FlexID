[include:common_16-S_parameter_inorganic.inc]

[parameter]
  # ICRP Publ.134 p.104 Table 5.3 / Ingested materials, All other forms

$if parent
  ${S}/fA    = 0.99 # ≒1
  ${S}/fA_Re = fA_MaxValueOfIng     # footnote††
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_16-S_inorganic.inc]
[include:element_17-Cl_inorganic.inc]
