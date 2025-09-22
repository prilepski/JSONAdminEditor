import { execSync } from 'child_process';

try {
  console.log('Generating TypeScript types from OpenAPI spec...');
  
  execSync('npx openapi-typescript http://localhost:8080/openapi/v1.json -o src/generated/api.ts', {
    stdio: 'inherit'
  });
  
  console.log('Types generated successfully!');
} catch (error) {
  console.error('Error generating types:', error.message);
  process.exit(1);
}