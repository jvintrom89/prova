#!/bin/bash

# Verifiquem si s'ha proporcionat un fitxer com a argument
if [ $# -ne 1 ]; then
  echo "Siusplau, proporcioneu el nom del fitxer com a argument."
  exit 1
fi

fitxer=$1

# Executem lsof per obtenir la llista de processos que utilitzen el fitxer
lsof_result=$(lsof "$fitxer" 2>/dev/null)

# Comprovem si lsof ha retornat alguna sortida
if [ -z "$lsof_result" ]; then
  echo "No hi ha processos utilitzant el fitxer $fitxer."
  exit 0
fi

# Iterem sobre les línies de lsof per obtenir els PID dels processos i fer kill
while IFS= read -r line; do
  pid=$(echo "$line" | awk '{print $2}')
  echo "Matant procés $pid..."
  kill "$pid"
done <<< "$lsof_result"

echo "Totes les instàncies que utilitzen el fitxer $fitxer han estat eliminades."
