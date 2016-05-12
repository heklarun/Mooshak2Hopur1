//
//  Matrix.cpp
//  Verkefni 1
//
//  Created by Sandra Ösp Stefánsdóttir on 13.1.2016.
//  Copyright © 2016 Sandra Ösp Stefánsdóttir. All rights reserved.
//

#include "Matrix.hpp"
#include <iostream>

Matrix::Matrix()
{
    
}

ostream& operator <<(ostream& out, const Matrix& m) // output
{
    for (int i = 0; i < m.row; i++)
    {
        for (int j = 0; j < m.col; j++)
        {
            out << m.arr[i][j] << "\t";
        }
        
        out << endl;
    }
    
    return out;
}

istream& operator >>(istream& in, Matrix& m) // input
{
    for (int i = 0; i < m.row; i++)
    {
        for (int j = 0; j < m.col; j++)
        {
            in >> m.arr[i][j];
        }
    }
    
    return in;
}

Matrix:: Matrix(int a, int b) // núllstilla
{
    row = a; 
    col = b;
    
    for (int i = 0; i < a; i++)
    {
        for (int j = 0; j < b; j++)
        {
            arr[i][j] = 0;
        }
    }
}

Matrix operator +(const Matrix& m1, const Matrix& m2) //Samlagning fylkja, m1 = A  og m2 = B
{
    Matrix temp(m1.row, m1.col);
 
    for(int i = 0; i < m1.row; i++)
    {
        for (int j = 0; j < m2.col; j++)
        {
            temp.arr[i][j] = m1.arr[i][j] + m2.arr[i][j];
        }
        
        cout << endl;
    }
    
    return temp;
}

Matrix operator *(const Matrix& m1, const Matrix& m2) //Margföldun fylkja, m1 = B  og m2 = C
{
    Matrix sinnum(m1.row, m1.col);
    
    for(int i = 0; i < m1.col; i++) //Dálkur niður
    {
        for (int j = 0; j < m2.row; j++) //Röð til hliðar
        {
            for (int k = 0; k < m2.col; k++) //Dálkur niður
            {
                sinnum.arr[i][j] += m1.arr[i][k] * m2.arr[k][j];
            }
        }
    }
    
    return sinnum;
}

Matrix Matrix::transpose()  // snýst við niður/upp
{
    Matrix temp(col, row); //Nýja array-ið (D)
    
    for(int i = 0; i < col; i++)
    {
        for (int j = 0; j < row; j++)
        {
            temp.arr[i][j] = arr[j][i];
        }
    }
    
    return temp;
}