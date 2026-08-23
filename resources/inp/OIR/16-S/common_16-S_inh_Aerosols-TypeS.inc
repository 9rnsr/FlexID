[include:common_16-S_parameter_inorganic.inc]

[parameter]
  # ICRP Publ.134 p.104 Table 5.3 / Inhaled particulate materials, Absorption type F

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote†
  $sb = 0           # footnote†

$if parent
  ${S}/fA    = 0.01
  ${S}/fA_Re = fA_MaxValueOfIng     # footnote††
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:common_intake_inh_Aerosols.inc]

[include:common_HRTM.inc]
[include:common_HATM+UB.inc]

[include:element_16-S_inorganic.inc]
[include:element_17-Cl_inorganic.inc]
