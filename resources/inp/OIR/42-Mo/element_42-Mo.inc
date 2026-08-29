[{Mo}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   Liver                 Liver
  acc   UrinaryPath           Kidneys
  acc   OtherKidneyTis        Kidneys
  acc   OtherTissue           Other         # ICRP Publ.134 p.310 Includes bone and all soft tissues other than liver and kidneys.


[{Mo}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.310 Table 14.3
  Blood1                  Blood2                   12.5
  Blood1                  Liver                    14.2
  Blood1                  UB-con                    6.5
  Blood2                  UrinaryPath               1.7
  Blood2                  OtherKidneyTis            0.115
  Blood2                  OtherTissue               1.73
  Liver                   RC-con                    0.0048
  Liver                   Blood2                    0.0122
  OtherKidneyTis          Blood2                    0.0474
  OtherTissue             Blood2                    0.0323
  UrinaryPath             UB-con                    1.40

$if progeny

  {Mo}/Blood1             Blood1                    ---
  {Mo}/Blood2             Blood2                    ---

  {Mo}/Liver              Liver                     ---
  {Mo}/UrinaryPath        UrinaryPath               ---
  {Mo}/OtherKidneyTis     OtherKidneyTis            ---

  {Mo}/OtherTissue        OtherTissue               ---

$endif
