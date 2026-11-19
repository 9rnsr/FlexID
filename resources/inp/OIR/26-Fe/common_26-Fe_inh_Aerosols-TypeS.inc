[include:common_26-Fe_parameter.inc]

[parameter]
  # ICRP Publ.134 p.136 Table 7.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Fe}/fA    = 0.001
  ${Fe}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
[include:element_25-Mn.inc]
