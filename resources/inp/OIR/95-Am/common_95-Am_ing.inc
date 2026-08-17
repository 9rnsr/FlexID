[include:common_95-Am_parameter.inc]

[parameter]
  # ICRP Publ.141 p.382 Table 23.5 / Ingested materials, All compounds

$if parent
  ${Am}/fA     = 5E-4
  ${Am}/fA_Re  = fA_MaxValueOfIng   # footnote**
$else
  ${^Rn}/fA    = fA_MaxValueOfIng
  ${^Rn}/fA_Re = fA_MaxValueOfIng
$endif

[include:common_intake_ing.inc]

[include:common_HATM+UB.inc]

[include:element_96-Cm.inc]
[include:element_95-Am.inc]
[include:element_94-Pu.inc]
[include:element_93-Np.inc]
[include:element_92-U.inc]
[include:element_91-Pa.inc]
[include:element_90-Th.inc]
[include:element_89-Ac.inc]
[include:element_88-Ra.inc]
[include:element_87-Fr.inc]
[include:element_86-Rn_ing.inc]
[include:element_85-At.inc]
[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
