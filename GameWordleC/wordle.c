#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <ctype.h>

#define MAX_WORDS 10000         // 单词数量
#define MAX_WORD_LENGTH 6       // 单词长度

// char word_list[MAX_WORDS][MAX_WORD_LENGTH];  // 二维数组长度有限，动态分配更优
static char** word_list;        // 存储单词列表 
static int word_count = 0;      // 记录单词数量

void loadwords(const char* filename) {

    // 打开文件，如果失败则打印错误信息并退出程序
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("error name\n");
        exit(1);
    }

    // 定义一个字符数组，用于存储从文件中读取的单词
    char buffer[MAX_WORD_LENGTH];

    // 动态分配内存，创建二维字符数组，用于存储单词列表（可以解决“对齐值超过了段的最大对齐值“）
    word_list = malloc(MAX_WORDS * sizeof(char*));
    if (word_list == NULL) {
        printf("Memory allocation failed\n");
        exit(1);
    }
    for (int i = 0; i < MAX_WORDS; i++) {
        word_list[i] = malloc(MAX_WORD_LENGTH * sizeof(char));
        if (word_list[i] == NULL) {
            printf("Memory allocation failed\n");
            exit(1);
        }
    }
    
    // 从文件中读取单词，存储到 word_list 中，并统计单词数量
    while (fgets(buffer, sizeof(buffer), file)!= NULL && word_count < MAX_WORDS) {
        
        buffer[strcspn(buffer, "\n")] = '\0';
        
        for (int i = 0; buffer[i]; i++) {
            buffer[i] = toupper(buffer[i]);
        }
        if(strlen(buffer) == 5) {
            strcpy(word_list[word_count], buffer);
            word_count++;
            printf("Loaded word: %s\n", buffer);
        }
    }
    fclose(file);
}

// 检查输入的单词是否在单词列表中，如果在则返回 1，否则返回 0
int is_valid_word(const char *word) {
    for (int i = 0; i < word_count; i++) {
        if (strcmp(word_list[i], word) == 0) {
            return 1;
        }
    }
    return 0;
}

void evaluate_guess(char *secret, char *guess, char *result) {
    // 获取秘密单词和猜测单词的长度，并初始化两个数组，用于标记匹配情况
    int secret_len = strlen(secret);
    int guess_len = strlen(guess);
    int secret_matched[secret_len];
    int guess_matched[guess_len];

    // 标记在相同位置匹配的字符
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
    // printf("Total words loaded: %d\n", word_count);

    while(1) {
        play_game();
        printf("play again?");
    }

    return 0;
}


