#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <ctype.h>

#define MAX_WORDS 1000
#define MAX_WORD_LENGTH 6

char word_list[MAX_WORDS][MAX_WORD_LENGTH];
int word_count = 0;

void loadwords(const char* filename) {
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("error name\n");
        exit(1);
    }

    char buffer[MAX_WORD_LENGTH];
    while (fgets(buffer, sizeof(buffer), file)!= NULL && word_count < MAX_WORDS) {
        buffer[strcspn(buffer, "\n")] = '\0';
        for (int i = 0; buffer[i]; i++) {
            buffer[i] = toupper(buffer[i]);
        }
        if(strlen(buffer)==5) {
            strcpy(word_list[word_count], buffer);
            word_count++;
        }
    }
    fclose(file);
}

int is_valid_word(const char *word) {
    for (int i = 0; i < word_count; i++) {
        if (strcmp(word_list[i], word) == 0) {
            return 1;
        }
    }
    return 0;
}

void evaluate_guess(char *secret, char *guess, char *result) {

    int secret_len = strlen(secret);
    int guess_len = strlen(guess);
    int secret_matched[secret_len];
    int guess_matched[guess_len];

    for (int i = 0; i < secret_len; i++) secret_matched[i] = 0;
    for (int i = 0; i < guess_len; i++) guess_matched[i] = 0;
    for (int i = 0; i < guess_len && i < secret_len; i++) {
        if (guess[i] == secret[i]) {
            result[i] = 'G';
            secret_matched[i] = 1;
            guess_matched[i] = 1;
        }
    }

    for(int i = 0; i < guess_len; i++) {
        if (guess_matched[i]) continue;
        for (int j = 0; j < secret_len; j++) {
            if (secret_matched[j]) continue;
            if (guess[i] == secret[j]) {
                result[i] = 'Y';
                secret_matched[i] = 1;
                guess_matched[i] = 1;
                break;
            } 
        }

        if (!guess_matched[i]) {
            result[i] = 'B';
        }

    }
    result[guess_len] = '\0';
}

void play_game() {
    char secret[MAX_WORD_LENGTH];
    strcpy(secret, word_list[rand() % word_count]);

    while(1) {
        printf("your guess: ");
        char guess[MAX_WORD_LENGTH];
        if (scanf("%5s", guess) != 1) {
            while (getchar() != '\n');
            continue;
        }

        for (int i = 0; guess[i]; i++) {
            guess[i] = toupper(guess[i]);
        }
        if (strlen(guess) != 5) {
            continue;
        }
        if (!is_valid_word(guess)) {
            continue;
        }
        char result[MAX_WORD_LENGTH];
        evaluate_guess(secret, guess, result);
        printf("result: %s\n", result);
        if(strcmp(result, "GGGGG") == 0) {
            printf("right: %s\n!", secret);
            break;
        }
    }
}

int main() {
    srand(time(NULL));
    loadwords("words.txt");
    if (word_count == 0) {
        printf("no word\n");
        return 1;
    }

    while(1) {
        play_game();
        printf("play again?");
    }

    return 0;

}

